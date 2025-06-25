import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { CadastroPetService } from '../../services/cadastro-pet.service';
import { Observable } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faTrash, faCalendar, faEye, faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDeleteComponent } from '../confirm-delete/confirm-delete.component';
import { EditarPetComponent } from '../editar-pet/editar-pet.component';
import { ViewPetComponent } from '../view-pet/view-pet.component';

interface Pet {
  id: number;
  nome: string;
  especie: string;
  tutor: string;
  emailTutor: string;
  raca: string;
  dataNascimento: string;
  peso: number;
  cor: string;
  descricao: string;
  imagem: string;
}

@Component({
  selector: 'app-pets',
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatDividerModule,
    MatPaginatorModule,
    FontAwesomeModule,
    FormsModule
  ],
  templateUrl: './pets.component.html',
  styleUrls: ['./pets.component.css']
})

export class PetsComponent implements OnInit {
  pets: Pet[] = [];
  paginacao: Pet[] = [];
  isNotFound: boolean = false;

  // Paginação
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 20];
  totalPets = 0;

  // Filtros
  searchType: string = 'todos';
  searchValue: string = '';

  constructor(private petService: CadastroPetService, private dialog: MatDialog) { }

  ngOnInit(): void {
    this.carregarPets();
  }

  carregarPets(): void {
    this.petService.listarPets().subscribe({
      next: (pets) => {
        if (Array.isArray(pets) && pets.length > 0) {
          this.pets = pets;
          this.isNotFound = false;
        } else {
          this.pets = [];
          this.isNotFound = true;
        }

        this.totalPets = pets.length;
        this.updatePaginatedPets();
      },
      error: (err) => {
        console.error('Erro ao carregar pets:', err);
        this.pets = [];
        this.isNotFound = true;
        this.totalPets = 0;
        this.updatePaginatedPets();
      }
    });
  }

  filtrarPets(event: Event): void {
    event.preventDefault();

    if (!this.searchValue.trim()) {
      this.isNotFound = false;
      this.carregarPets();
      return;
    }

    let filtro: Observable<Pet[]>;

    switch (this.searchType) {
      case 'todos':
        filtro = this.petService.listarPets();
        break;
      case 'especie':
        filtro = this.petService.listarPetPorEspecie(this.searchValue);
        break;

      case 'raca':
        filtro = this.petService.listarPetPorRaca(this.searchValue);
        break;

      case 'id':
        const id = Number(this.searchValue);
        filtro = this.petService.listarPetPorId(id);
        break;

      default:
        return;
    }

    filtro.subscribe({
      next: (pets) => {
        this.isNotFound = false;
        if (Array.isArray(pets)) {
          this.pets = pets;
        } else {
          this.pets = [pets];
        }
        this.totalPets = pets.length;
        this.updatePaginatedPets();
      },
      error: (err) => {
        console.error('Erro ao filtrar pets:', err);

        if (err.status === 404) {
          this.isNotFound = true;
        } else {
          this.isNotFound = false;
        }

        this.pets = [];
        this.totalPets = 0;
        this.updatePaginatedPets();
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePaginatedPets();
  }

  private updatePaginatedPets(): void {
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginacao = this.pets.slice(startIndex, endIndex);
  }

  // ícones
  faCalendar = faCalendar;
  faPen = faPen;
  faTrash = faTrash;
  faEye = faEye;
  faMagnifyingGlass = faMagnifyingGlass;

  deletarPet(id: number): void {
    this.dialog.open(ConfirmDeleteComponent).afterClosed().subscribe((result) => {
      if (result) {
        this.petService.DeletarPetPorId(id).subscribe({
          next: () => this.carregarPets(),
          error: (err) => console.error('Erro ao deletar o pet:', err)
        });
      }
    });
  }
  
  EditarPet(pet: any): void {
    const dialogRef = this.dialog.open(EditarPetComponent, {
      data: pet 
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Pet editado com sucesso!');
      }
      this.carregarPets();
    });
  }

  verPet(pet: any): void {
    const dialogRef = this.dialog.open(ViewPetComponent, {
      data: pet 
    });

  }

}