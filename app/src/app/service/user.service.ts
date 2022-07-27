import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../models/response.model';

@Injectable({
    providedIn: 'root'
})
export class UserService {

    ENDPOINT_CHANGE_MY_PASSWORD = 'user/changemypassword';
    USERINFO_KEY = 'userinfo';
    ENDPOINT_AUTHEN_GETUSERINFO = 'user/getuserinfo';

    constructor(
        private _httpClient: HttpClient
    ) { }

    changeMyPassword(newPassword: string): Promise<ResponseModel> {
        const url = `${environment.apiEndpoint}/${this.ENDPOINT_CHANGE_MY_PASSWORD}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, { newPassword }));
    }

    getAll(): Promise<any> {
        const url = `${environment.apiEndpoint}/user`;
        return firstValueFrom(this._httpClient.get<ResponseModel>(url));
    }

    getById(id: string): Promise<any> {
        const url = `${environment.apiEndpoint}/user/${id}`;
        return firstValueFrom(this._httpClient.get<ResponseModel>(url));
    }

    create(user: any): Promise<any> {
        const url = `${environment.apiEndpoint}/user`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, user));
    }

    update(user: any): Promise<any> {
        const url = `${environment.apiEndpoint}/user`;
        return firstValueFrom(this._httpClient.put<ResponseModel>(url, user));
    }

    delete(id: string): Promise<any> {
        const url = `${environment.apiEndpoint}/user/${id}`;
        return firstValueFrom(this._httpClient.delete<ResponseModel>(url));
    }

    async getUserInfo(): Promise<any> {
        const existData = localStorage.getItem(this.USERINFO_KEY);
        if (existData) {
            return JSON.parse(existData);
        } else {
            const url = `${environment.apiEndpoint}/${this.ENDPOINT_AUTHEN_GETUSERINFO}`;
            const rs = await firstValueFrom(this._httpClient.get<ResponseModel>(url));
            if (rs.success) {
                localStorage.setItem(this.USERINFO_KEY, JSON.stringify(rs.data));
                return rs.data;
            }
        }
    }

    async hasManagePermission() {
        const userInfo = await this.getUserInfo();
        return userInfo.userName == 'root';
    }
}
