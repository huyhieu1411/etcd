import { Component, Input, OnInit, Output } from '@angular/core';
import { MenuItem, Message, MessageService } from 'primeng/api';
import { Dialog } from 'primeng/dialog';
import { UserService } from 'src/app/service/user.service';

@Component({
  selector: 'app-change-my-password',
  templateUrl: './change-my-password.component.html',
  styleUrls: ['./change-my-password.component.scss']
})
export class ChangeMyPasswordComponent implements OnInit {

  @Input() dialog: Dialog;

  newPassword = '';
  retypeNewPassword = '';
  msg: Message[] = [];
  processing = false;
  public buttons: MenuItem[];

  constructor(
    private _userSevice: UserService,
    private _messageService: MessageService
  ) { }

  ngOnInit() {
    this.buttons = [
      {
        label: 'Cancel',
        icon: 'pi pi-times',
        disabled: this.processing,
        command: () => {
          this.dialog.close({} as Event);
        }
      },
      {
        label: 'Save',
        icon: 'pi pi-check',
        disabled: this.processing,
        command: this.saveChangePassword.bind(this)
      }
    ]
  }

  saveChangePassword() {
    if (!this.newPassword) {
      this.msg.push({ severity: 'error', summary: 'Error Message', detail: 'Please enter new password' });
      return;
    }
    if (this.newPassword !== this.retypeNewPassword) {
      this.msg.push({ severity: 'error', summary: 'Error Message', detail: 'New password and retype new password is not match' });
      return;
    }
    this.processing = true;
    this._userSevice.changeMyPassword(this.newPassword).then(rs => {
      if (rs.status === 200) {
        this._messageService.add({ severity: 'success', summary: 'Success Message', detail: 'Change password success' });
        this.dialog.close({} as Event);
      } else {
        this.msg.push({ severity: 'error', summary: 'Error Message', detail: rs.message });
        this.processing = false;
      }
    }, err => {
      this.msg.push({ severity: 'error', summary: 'Error Message', detail: err.message });
      this.processing = false;
    }
    );
  }
}
