import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../models/response.model';
import { AppCtxService } from './app-ctx.service';
import { ComCtxService } from './com-ctx.service';
import { ConnectionService } from './connection.service';

@Injectable({
    providedIn: 'root'
})
export class KeyValueService {
    readonly ENDPOINT_GET_KEY_BY_CONNECTION = 'keyvalue/getallkeys';
    readonly ENDPOINT_GET_ALL_BY_CONNECTION = 'keyvalue/getall';
    readonly ENDPOINT_GET_DETAIL_BY_KEY = 'keyvalue/get';
    readonly ENDPOINT_DELETE_BY_KEY = 'keyvalue/delete';
    readonly ENDPOINT_SAVE = 'keyvalue/save';
    readonly ENDPOINT_RENAME_KEY = 'keyvalue/renamekey';
    readonly rootCtx: ComCtxService;
    constructor(
        private _httpClient: HttpClient,
        private _confirmationService: ConfirmationService,
        private _messageService: MessageService,
        private _appCtxService: AppCtxService,
        private _connectionService: ConnectionService
    ) {
        this.rootCtx = this._appCtxService.getRootCtx();
    }

    getByKeyPrefix(key: string): Promise<ResponseModel> {
        const url = `${this._connectionService.getAgentDomain()}/keyvalue/GetByKeyPrefix?keyPrefix=${encodeURIComponent(key)}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, this.rootCtx.data.connection));
    }

    getAll() {
        const url = `${this._connectionService.getAgentDomain()}/${this.ENDPOINT_GET_ALL_BY_CONNECTION}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, this.rootCtx.data.connection));
    }

    getAllKeys(): Promise<ResponseModel> {
        const url = `${this._connectionService.getAgentDomain()}/${this.ENDPOINT_GET_KEY_BY_CONNECTION}`;
        return firstValueFrom(this._httpClient.post<any>(url, this.rootCtx.data.connection));
    }

    getByKey(key: string): Promise<ResponseModel> {
        const url = `${this._connectionService.getAgentDomain()}/${this.ENDPOINT_GET_DETAIL_BY_KEY}?key=${encodeURIComponent(key)}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, this.rootCtx.data.connection));
    }

    save(keyModel: any, isInsert = false): Promise<ResponseModel> {
        const url = `${this._connectionService.getAgentDomain()}/${this.ENDPOINT_SAVE}`;
        keyModel.isInsert = isInsert;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, { ...keyModel, connection: this.rootCtx.data.connection }));
    }

    delete(key: string, deleteRecursive = false): Promise<ResponseModel> {
        const url = `${this._connectionService.getAgentDomain()}/${this.ENDPOINT_DELETE_BY_KEY}?key=${encodeURIComponent(key)}&deleteRecursive=${deleteRecursive}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, this.rootCtx.data.connection));
    }

    onDelete(key: string, deleteRecursive = false): Promise<any> {
        return new Promise((resolve) => {
            this._confirmationService.confirm({
                message: 'Are you sure that you want to delete?',
                accept: () => {
                    this.delete(key, deleteRecursive).then(rs => {
                        if (rs.success) {
                            resolve(true);
                            this.rootCtx.dispatchEvent('keyDeleted', { key });
                            this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Delete success' });
                        } else {
                            this._messageService.add({ severity: 'error', summary: 'Error', detail: rs.message });
                            resolve(false);
                        }
                    }).catch(err => {
                        resolve(false);
                        this._messageService.add({ severity: 'error', summary: 'Error', detail: err.message });
                    });
                },
            });
        })
    }

    isRootKey(key: string): boolean {
        return key === '/';
    }

    renameKey(oldKey: string, newKey: string): Promise<ResponseModel> {
        const url = `${this._connectionService.getAgentDomain()}/${this.ENDPOINT_RENAME_KEY}?oldKey=${encodeURIComponent(oldKey)}&newKey=${encodeURIComponent(newKey)}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, this.rootCtx.data.connection));
    }

    getRevisionOfKey(key: string): Promise<ResponseModel> {
        const url = `${this._connectionService.getAgentDomain()}/keyvalue/getrevision?key=${encodeURIComponent(key)}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, this.rootCtx.data.connection));
    }

    importNodes(nodes: any[]): Promise<ResponseModel> {
        const url = `${this._connectionService.getAgentDomain()}/keyvalue/importnodes`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, { connection: this.rootCtx.data.connection, keyModels: nodes }));
    }
}
