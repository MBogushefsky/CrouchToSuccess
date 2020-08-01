// Credits to https://blog.angulartraining.com/http-interceptors-in-angular-61dcf80b6bdd
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from "rxjs";
import { Globals } from '../globals';

@Injectable()
export class HttpInterceptorService implements HttpInterceptor {
   constructor(private globals: Globals) {}

   intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      let newHeaders = req.headers;
      if (!req.url.includes('yodlee')) {
         if (this.globals.userToken != null){
            newHeaders = newHeaders.append('Authorization', 'Basic ' + btoa(this.globals.userToken.Username + ":" + this.globals.userToken.PasswordHash));
         }
         newHeaders = newHeaders.append('Accept-Language', 'en-US');
         newHeaders = newHeaders.append('Content-Type', 'application/json');
      }
      const authReq = req.clone({ headers: newHeaders });
      return next.handle(authReq);
   }
}