import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConfirmEmailComponent } from './account/confirm-email/confirm-email.component';
import { ResetPasswordComponent } from './account/reset-password/reset-password.component';
import { BadRequestComponent } from './errors/bad-request/bad-request.component';
import { InternalServerComponent } from './errors/internal-server/internal-server.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { UnauthorizedComponent } from './errors/unauthorized/unauthorized.component';
import { FailedComponent } from './payment/failed/failed.component';
import { SuccessComponent } from './payment/success/success.component';

const routes: Routes = [
  {path: 'payment/success', component: SuccessComponent},
  {path: 'payment/failed', component: FailedComponent},
  {path: 'account/resetpassword', component:ResetPasswordComponent},
  {path: 'account/confirmemail', component:ConfirmEmailComponent},
  {path: 'error/notfound', component: NotFoundComponent},
  {path: 'error/badrequest', component:BadRequestComponent},
  {path: 'error/internal', component:InternalServerComponent},
  {path: 'error/unauthorized', component:UnauthorizedComponent},
  {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
