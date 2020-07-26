import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/models';
import * as shajs from 'sha.js';

@Injectable({
  providedIn: 'root'
})
export class MonierService {
  private apiUrl = 'http://localhost';

  constructor(private http: HttpClient) { }

  getUserByCredentials(username: string, passwordHash: string) {
    return this.http.get(this.apiUrl + '/user?Username=' + username + '&PasswordHash=' + encodeURIComponent(shajs('sha256').update({passwordHash}).digest('hex')));
  }

  createUser(userToCreate: User) {
    return this.http.post(this.apiUrl + '/user', userToCreate);
  }

  syncAllBankAccounts(){
    return this.http.put(this.apiUrl + '/sync/everything', {});
  }

  getAllBankAccounts(){
    return this.http.get(this.apiUrl + '/bankAccount');
  }

  getBankAccountById(id: string){
    return this.http.get(this.apiUrl + '/bankAccount/' + id);
  }

  getOverallStatistics(){
    return this.http.get(this.apiUrl + '/statistics');
  }
  
  getTransactionsByPlaidBankAccountId(plaidBankAccountId: string){
    return this.http.get(this.apiUrl + '/transaction/' + plaidBankAccountId);
  }

  linkBankAccountToUser(publicToken: string){
    return this.http.post(this.apiUrl + '/user/linkBank', '"' + publicToken + '"');
  }

  deleteLinkToBankAccount(bankAccountId: string){
    return this.http.delete(this.apiUrl + '/user/linkBank/' + bankAccountId);
  }

}
