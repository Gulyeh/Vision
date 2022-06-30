import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmParams } from 'src/app/_models/ConfirmParams';
import { AccountService } from 'src/app/_services/account.service';
import { BusyService } from 'src/app/_services/busy.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {
  confirmEmailModel: ConfirmParams = {UserId: '', Token: ''};
  IsBusy = true;
  responseString = '';

  constructor(private accountService: AccountService, private route: ActivatedRoute, private router: Router, private busyService: BusyService) { 
    this.IsBusy = busyService.IsBusy;
    this.route.queryParams.subscribe(params => {
      this.confirmEmailModel.UserId = params["userId"];
      this.confirmEmailModel.Token = params["token"];
    });
  }

  ngOnInit(): void {
      if(this.confirmEmailModel.Token === '' || this.confirmEmailModel.UserId === ''){
        this.router.navigateByUrl('error/badrequest');
        return;
      }
      
      this.accountService.confirmEmail(this.confirmEmailModel).subscribe({
        next: response => {
            this.responseString = response.response;
        }
      })
  }

}
