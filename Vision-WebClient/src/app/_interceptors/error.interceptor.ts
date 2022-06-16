import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, of, throwError } from 'rxjs';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
          if(error){
            switch(error.status){
              case 404:
                this.router.navigateByUrl('error/notfound');
                break;
              case 401:
                this.router.navigateByUrl('error/unauthorized');
                break;
              case 400:
                this.router.navigateByUrl('error/badrequest');
                break;
              case 500:
                this.router.navigateByUrl('error/internal');
                break;
              default:
                break;
            }
          }
          return throwError(() => error);
      })
    );
  }
}
