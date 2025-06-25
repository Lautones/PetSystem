import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

interface raca {
    id: number,
    nome: string
}

@Injectable({
    providedIn: 'root'
})
export class CadastroPetService {
    private apiUrl = 'https://localhost:5001/api';

    constructor(private http: HttpClient) { }

    cadastrarPet(petData: {
        Nome: string,
        Especie: string,
        Tutor: string,
        EmailTutor: string,
        Raca: string,
        DataNascimento: string,
        Peso: number,
        Cor: string,
        Descricao?: string;
    }): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/Pet`, petData);
    }

    listarPets(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/Pet`);

    }

    listarRacasCaes(): Observable<string[]> {
        return this.http.get<raca[]>(`${this.apiUrl}/racas/cachorro`)
            .pipe(map(resposta => resposta.map(raca => raca.nome)));

    }

    listarRacasGatos(): Observable<string[]> {
        return this.http.get<raca[]>(`${this.apiUrl}/racas/gato`)
            .pipe(map(resposta => resposta.map(raca => raca.nome)));

    }

    listarPetPorId(id: number): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/Pet/${id}`);
    }

    listarPetPorRaca(raca: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/Pet/raca/${raca}`);
    }

    listarPetPorEspecie(especie: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/Pet/especie/${especie}`);
    }

    DeletarPetPorId(id: number): Observable<any> {
        return this.http.delete<any>(`${this.apiUrl}/Pet/${id}`)
    }

    editarPet(id: number, petData: {
        Nome: string;
        Especie: string;
        Tutor: string;
        EmailTutor: string;
        Raca: string;
        DataNascimento: string;
        Peso: number;
        Cor: string;
        Descricao?: string;
    }): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/Pet?id=${id}`, petData);
    }

    listarIds(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/Pet/ids`);

    }

}
