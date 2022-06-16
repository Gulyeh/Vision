import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  requestCount = 0;
  IsBusy!: boolean;
  constructor(private spinner: NgxSpinnerService) { }

  busy(){
    this.IsBusy = true;
    this.requestCount++;
    this.spinner.show(undefined, {
      type: 'line-scale-party',
      bdColor: "rgba(255, 255, 255, 0)",
      color: "#333333"
    });
  }

  idle()
  {
    this.requestCount--;
    if(this.requestCount <= 0){
      this.IsBusy = false;
      this.requestCount = 0;
      this.spinner.hide();
    }
  }
}
