import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Response } from '../_models/apiResponse';
import { map } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { PaymentCompleted } from '../_models/PaymentCompleted';
import { PaymentData } from '../_models/PaymentData';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  paymentSuccess(model: PaymentCompleted){
    return this.http.post<Response<PaymentData>>(this.baseUrl + '/api/payment/success', model);
  }

  paymentFailed(model: PaymentCompleted){
    return this.http.post<Response<PaymentData>>(this.baseUrl + '/api/payment/failed', model);
  } 
}
