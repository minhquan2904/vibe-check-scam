import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CheckScamRequest, CheckScamResponse } from '../models/scam.model';

@Injectable({
  providedIn: 'root'
})
export class ScamService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5193/api/v1/check-scam';

  checkScam(rawText: string): Observable<CheckScamResponse> {
    return this.http.post<CheckScamResponse>(this.apiUrl, { rawText });
  }
}
