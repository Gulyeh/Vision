import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { SuccessComponent } from './payment/success/success.component';
import { FailedComponent } from './payment/failed/failed.component';
import { ResetPasswordComponent } from './account/reset-password/reset-password.component';
import { ConfirmEmailComponent } from './account/confirm-email/confirm-email.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { NavComponent } from './nav/nav.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BadRequestComponent } from './errors/bad-request/bad-request.component';
import { InternalServerComponent } from './errors/internal-server/internal-server.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { UnauthorizedComponent } from './errors/unauthorized/unauthorized.component';

@NgModule({
  declarations: [
    AppComponent,
    NotFoundComponent,
    SuccessComponent,
    FailedComponent,
    ResetPasswordComponent,
    ConfirmEmailComponent,
    NavComponent,
    BadRequestComponent,
    InternalServerComponent,
    UnauthorizedComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    NgxSpinnerModule,
    NgbModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
