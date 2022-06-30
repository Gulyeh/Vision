import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Response } from '../_models/apiResponse';
import { HttpClient } from '@angular/common/http';
import { ResetPassword } from '../_models/ResetPassword';
import { ConfirmParams } from '../_models/ConfirmParams';

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  resetPassword(model: ResetPassword){
    return this.http.post<Response<string>>(this.baseUrl + '/api/account/resetpassword', model);
  }

  checkUserExists(user: string){
    return this.http.get<Response<boolean>>(this.baseUrl + '/api/users/UserExists?userId=${user}');
  }

  confirmEmail(model: ConfirmParams){
    return this.http.post<Response<string>>(this.baseUrl + '/api/account/confirmEmail', model);
  }
}
