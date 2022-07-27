import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ResponseModel } from 'src/app/models/response.model';
import { AppCtxService } from 'src/app/service/app-ctx.service';
import { ComCtxService } from 'src/app/service/com-ctx.service';
import { ConnectionService } from 'src/app/service/connection.service';
import { UserService } from 'src/app/service/user.service';

@Component({
    selector: 'app-user-manager',
    templateUrl: './user-manager.component.html',
    styleUrls: ['./user-manager.component.scss']
})
export class UserManagerComponent implements OnInit {
    loading = false;
    processing = false;
    users = [];
    formState: 'list' | 'new' | 'edit' = 'list';
    form: FormGroup;
    msgs = [];
    rootCtx: ComCtxService;
    connectionDataSource = [];
    @Output() closeForm: EventEmitter<void> = new EventEmitter<void>();

    constructor(
        private _confirmationService: ConfirmationService,
        private _messageService: MessageService,
        private _appCtxService: AppCtxService,
        private _userService: UserService,
        private _connectionService: ConnectionService
    ) {
        this.rootCtx = this._appCtxService.getRootCtx();
        this.form = new FormGroup({
            id: new FormControl(),
            userName: new FormControl('user1', { nonNullable: true, validators: [Validators.required] }),
            password: new FormControl(''),
            retypePassword: new FormControl(''),
            connectionPermissions: new FormControl('')
        });
    }

    ngOnInit() {
        this.loadGrid();
    }

    async loadGrid() {
        this.users = (await this._userService.getAll()).data;
    }

    newUser() {
        this.formState = 'new';
        this.form.patchValue({});
        this.loadConnectionDataSource();
    }

    cancel() {
        this.formState = 'list';
    }

    async loadConnectionDataSource() {
        let data = (await this._connectionService.getDataSource()).data;
        if (data) {
            data = data.map(x => {
                x.label = `${x.server} - ${x.userName}`;
                return x;
            });
        }
        this.connectionDataSource = data;
    }

    async saveUser() {
        if (this.form.valid) {
            if (this.formState == 'new') {
                if (this.form.value.password !== this.form.value.retypePassword) {
                    this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Password not match' });
                    return;
                }
            }

            this.processing = true;

            let result: ResponseModel;
            if (this.formState == 'new') {
                result = await this._userService.create(this.form.value);
            } else {
                result = await this._userService.update(this.form.value);
            }
            if (!result.success) {
                this._messageService.add({ severity: 'error', summary: 'Error', detail: result.error });
            } else {
                this.rootCtx.dispatchReplayEvent('RELOAD_DATASOURCE');
                this.formState = 'list';
                this.loadGrid();
            }
            this.processing = false;
        }
    }

    editUser(item: any) {
        this.formState = 'edit';
        this.loadConnectionDataSource();
        this.form.patchValue(item);
    }

    deleteUser(event: Event, item: any) {
        this._confirmationService.confirm({
            target: event.target,
            message: 'Are you sure you want to delete this connection?',
            accept: () => {
                this._userService.delete(item.id).then(rs => {
                    if (rs.success) {
                        this.rootCtx.dispatchReplayEvent('RELOAD_DATASOURCE');
                        this.loadGrid();
                    } else {
                        this._messageService.add({ severity: 'error', summary: 'Error', detail: rs.error });
                    }
                });
            },
        });
    }
}
