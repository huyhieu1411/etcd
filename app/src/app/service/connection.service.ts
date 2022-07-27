import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Guid } from 'guid-ts';
import { firstValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../models/response.model';
import { AppCtxService } from './app-ctx.service';
import { ComCtxService } from './com-ctx.service';

@Injectable({
    providedIn: 'root'
})
export class ConnectionService {

    readonly STORAGE_KEY = 'etcdmanager_data';
    readonly ENDPOINT_CHECK_CONNECTION = 'connection/checkconnection';
    readonly ENDPOINT_DELETE_BY_NAME = 'connection/deleteconnectionbyname';
    readonly ENDPOINT_CONNECTION = 'connection';
    selectedConnection: any;
    rootCtx: ComCtxService;

    constructor(
        private _httpClient: HttpClient,
        private _appCtxService: AppCtxService
    ) {
        this.rootCtx = this._appCtxService.getRootCtx();

        if (this.rootCtx) {
            this.rootCtx.replaySubscribe('SELECT_CONNECTION', (connection) => {
                this.selectedConnection = connection;
            });
        } else {
            console.warn('root ctx is null');
        }
    }

    setSelectedConnection(connection: any) {
        this.selectedConnection = connection;
    }

    getAgentDomain() {
        if (this.selectedConnection && this.selectedConnection.agentDomain && this.selectedConnection.agentDomain != '') {
            return this.selectedConnection.agentDomain;
        }
        return environment.apiEndpoint;
    }

    checkConnection(connection: any): Promise<ResponseModel> {
        const agentDomain = connection.agentDomain && connection.agentDomain != '' ? connection.agentDomain : environment.apiEndpoint;
        const url = `${agentDomain}/${this.ENDPOINT_CHECK_CONNECTION}`;
        if (!connection.id) {
            connection = { ...connection };
        }
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, connection));
    }

    update(connection: any): Promise<ResponseModel> {
        const url = `${environment.apiEndpoint}/${this.ENDPOINT_CONNECTION}/${connection.id}`;
        return firstValueFrom(this._httpClient.put<ResponseModel>(url, connection));
    }

    insert(connection: any): Promise<ResponseModel> {
        const url = `${environment.apiEndpoint}/${this.ENDPOINT_CONNECTION}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, connection));
    }

    validate(ds: any[], connection: any): string {
        if (ds.some(x => x.name == connection.name && x.id != connection.id && x.enableAuthenticated == connection.enableAuthenticated)) {
            return 'connection name already exists';
        }
        if (connection.enableAuthenticated) {
            const existItem = ds.find(x =>
                x.server == connection.server
                && x.userName == connection.userName
                && x.password == connection.password
                && x.enableAuthenticated == connection.enableAuthenticated
                && x.id != connection.id);
            if (existItem) {
                return 'connection with server, userName and password already exists with name "' + existItem.name + "\"";
            }
        } else {
            const existItem = ds.find(x => x.server == connection.server && x.enableAuthenticated == false);
            if (existItem) {
                return 'connection with server, userName and password already exists with name "' + existItem.name + "\"";
            }
        }
        return '';
    }

    deleteByName(name: string) {
        const url = `${environment.apiEndpoint}/${this.ENDPOINT_DELETE_BY_NAME}?name=${name}`;
        return firstValueFrom(this._httpClient.delete<ResponseModel>(url));
    }

    getByName(name: string): Promise<ResponseModel> {
        const url = `${environment.apiEndpoint}/${this.ENDPOINT_CONNECTION}/GetByName?name=${name}`;
        return firstValueFrom(this._httpClient.get<ResponseModel>(url));
    }

    getDataSource() {
        const url = `${environment.apiEndpoint}/${this.ENDPOINT_CONNECTION}`;
        return firstValueFrom(this._httpClient.get<ResponseModel>(url));
    }
}
