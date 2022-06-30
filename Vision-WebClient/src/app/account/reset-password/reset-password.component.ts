import { Component, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators  } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ResetPassword } from 'src/app/_models/ResetPassword';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit{
  resetPasswordForm: FormGroup = this.formBuilder.group({
    newPassword: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(15)]],
    confirmPassword: ['', [Validators.required]],
  },
    {
      validators: [this.match('newPassword', 'confirmPassword')]
    }
  );
  resetPasswordModel: ResetPassword = {UserId: '', Token: '', NewPassword: '', ConfirmNewPassword: ''};
  submitted = false;
  responseString = '';

  constructor(private accountService: AccountService, private formBuilder: FormBuilder, private router: Router, private route: ActivatedRoute) {
    this.route.queryParams.subscribe(params => {
      this.resetPasswordModel.UserId = params["userId"];
      this.resetPasswordModel.Token = params["token"];
    });
  }

  ngOnInit(): void {
    if(this.resetPasswordModel.Token === '' || this.resetPasswordModel.UserId === ''){
      this.router.navigateByUrl('error/badrequest');
      return;
    }    
  }


   match(controlName: string, checkControlName: string): ValidatorFn {
      return (controls: AbstractControl) => {
        const control = controls.get(controlName);
        const checkControl = controls.get(checkControlName);
        if (checkControl?.errors && !checkControl.errors['matching']) {
          return null;
        }
        if (control?.value !== checkControl?.value) {
          controls.get(checkControlName)?.setErrors({ matching: true });
          return { matching: true };
        } else {
          return null;
        }
      };
  }

  resetPassword(){
    this.submitted = true;
    this.resetPasswordModel.NewPassword = this.resetPasswordForm.get('newPassword')?.value;
    this.resetPasswordModel.ConfirmNewPassword = this.resetPasswordForm.get('confirmPassword')?.value;
    
    this.accountService.resetPassword(this.resetPasswordModel).subscribe({
      next: response => {
        this.responseString = response.response;
      }
    })
  }
}
