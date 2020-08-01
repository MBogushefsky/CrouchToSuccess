import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class YodleeService {
  //private apiUrl = 'https://sandbox.api.yodlee.com/ysl';
  private apiUrl = 'https://development.api.yodlee.com/ysl';
  private clientId = 'MXoJfWrxmoAGiYWy3iFG0vik3wqmvXD5';
  private secret = '4Qr4fnDeH4ZV1cw7';

  constructor(private http: HttpClient) { }

  getUserAccessToken(loginName: string) {
    let headers = new HttpHeaders({
      'Api-Version': '1.1',
      'loginName': loginName,
      'Content-Type': 'application/x-www-form-urlencoded'
    });
    let options = { headers: headers };
    return this.http.post(this.apiUrl + '/auth/token', 'clientId=' + this.clientId + '&secret=' + this.secret, options);
  }
}

