import { FormGroupDirective } from "@angular/forms";

export interface ResetPassword{
    UserId: string;
    Token: string;
    NewPassword: string;
    ConfirmNewPassword: string;
}