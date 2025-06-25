import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AgendamentosService {
  private apiUrl = 'http://localhost:5129/api/Agendamento';

  constructor(private http: HttpClient) { }

  agendar(agendamentoData: {
    petId: number;
    tipo: string;
    data: string;
    // hora: string;
    observacao?: string;
  }): Observable<any> {
    return this.http.post<any>(this.apiUrl, agendamentoData);
  }

  listarAgendamentos(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}`);
  }

  listarAgendamentoPorId(Petd: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${Petd}`);
  }

  DeletarAgendamentoPorId(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`)
  }

  editarAgendamento(id: number, agendamentoData: {
    petId: number;
    tipo: string;
    data: string;
    // hora: string;
    observacao?: string;
  }): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, agendamentoData);
  }

}
