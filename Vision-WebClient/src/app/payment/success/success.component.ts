import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmParams } from 'src/app/_models/ConfirmParams';
import { PaymentCompleted } from 'src/app/_models/PaymentCompleted';
import { PaymentData } from 'src/app/_models/PaymentData';
import { BusyService } from 'src/app/_services/busy.service';
import { PaymentService } from 'src/app/_services/payment.service';

@Component({
  selector: 'app-success',
  templateUrl: './success.component.html',
  styleUrls: ['./success.component.css']
})
export class SuccessComponent implements OnInit {
  paymentResponse!: PaymentData;
  paymentModel: PaymentCompleted = {SessionId: ''};
  IsBusy = true;

  constructor(private route: ActivatedRoute, private router: Router, private paymentService: PaymentService, private busyService: BusyService) { 
    this.IsBusy = busyService.IsBusy;
    this.route.queryParams.subscribe(params => {
      this.paymentModel.SessionId = params["sessionId"];
    });
  }

  ngOnInit(): void {
    if(this.paymentModel.SessionId === ''){
      this.router.navigateByUrl('error/badrequest');
      return;
    }   

    this.paymentService.paymentSuccess(this.paymentModel).subscribe({
      next: response => {
        if(!response.isSuccess){
          this.router.navigateByUrl('error/badrequest');
          return;
        }
        this.paymentResponse = response.response;
      }
    })
  }

}
