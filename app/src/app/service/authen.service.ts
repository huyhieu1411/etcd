import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseModel } from '../models/response.model';

@Injectable({
    providedIn: 'root'
})
export class AuthenService {

    TOKEN_KEY = 'access_token';
    USERINFO_KEY = 'userinfo';
    ENDPOINT_AUTHEN_LOGIN = 'authen/login';
    ENDPOINT_AUTHEN_LOGOUT = 'authen/logout';

    constructor(
        private _httpClient: HttpClient,
        private _router: Router
    ) {

    }

    hasValidAccessToken() {
        const token = this.getAccessToken();
        return token !== null && token !== undefined;
    }

    getAccessToken() {
        return localStorage.getItem(this.TOKEN_KEY);
    }

    login(userName: string, password: string): Promise<ResponseModel> {
        const url = `${environment.apiEndpoint}/${this.ENDPOINT_AUTHEN_LOGIN}`;
        return firstValueFrom(this._httpClient.post<ResponseModel>(url, { userName, password }));
    }

    localLogout() {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem(this.USERINFO_KEY);
        this._router.navigateByUrl('/login');
    }

    async logout() {
        const url = `${environment.apiEndpoint}/${this.ENDPOINT_AUTHEN_LOGOUT}`;
        const rs = await firstValueFrom(this._httpClient.post<ResponseModel>(url, null));
        if (rs.success || rs.error == 'Token not found') {
            localStorage.removeItem(this.TOKEN_KEY);
            localStorage.removeItem(this.USERINFO_KEY);
            this._router.navigateByUrl('/login');
        } else {
            throw new Error(rs.message);
        }
    }

    saveToken(token: string) {
        localStorage.setItem(this.TOKEN_KEY, token);
    }
}
