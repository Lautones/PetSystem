import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { Observable } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faTrash, faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { AgendamentosService } from '../../services/agendamentos.service';
import { ConfirmDeleteComponent } from '../confirm-delete/confirm-delete.component';
import { EditarAgendamentosComponent } from '../editar-agendamentos/editar-agendamentos.component';

interface Agendamento {
  id: number;
  petId: number;
  tipo: string;
  data: string;
  observacao: string;
}

@Component({
  selector: 'app-agendamentos',
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
  templateUrl: './agendamentos.component.html',
  styleUrl: './agendamentos.component.css'
})

export class AgendamentosComponent implements OnInit {

  agendamentos: Agendamento[] = [];
  paginacao: Agendamento[] = [];
  isNotFound: boolean = false;

  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 20];
  totalAgendamentos = 0;

  // Filtros
  searchType: string = 'todos';
  searchValue: string = '';

  constructor(private agendamentoService: AgendamentosService, private dialog: MatDialog) { }

  ngOnInit(): void {
    this.carregarAgendamentos();
  }

  carregarAgendamentos(): void {

    this.agendamentoService.listarAgendamentos().subscribe({
      next: (agendamentos) => {
        if (Array.isArray(agendamentos) && agendamentos.length > 0) {
          this.agendamentos = agendamentos;
          this.isNotFound = false;
        } else {
          this.agendamentos = [];
          this.isNotFound = true;
        }

        this.totalAgendamentos = this.agendamentos.length;
        this.updatePaginatedAgendamentos();
      },
      error: (err) => {
        console.error('Erro ao carregar agendamentos:', err);
        this.agendamentos = [];
        this.isNotFound = true;
        this.totalAgendamentos = 0;
        this.updatePaginatedAgendamentos();
      }
    });
  }

  filtrarAgendamentos(event: Event): void {
    event.preventDefault();

    if (!this.searchValue.trim()) {
      this.isNotFound = false;
      this.carregarAgendamentos();
      return;
    }

    let filtro: Observable<Agendamento[]>;

    switch (this.searchType) {
      case 'todos':
        filtro = this.agendamentoService.listarAgendamentos();
        break;
      case 'petId':
        const petId = Number(this.searchValue);
        filtro = this.agendamentoService.listarAgendamentoPorId(petId);
        break;

      default:
        return;
    }

    filtro.subscribe({
      next: (agendamentos) => {
        this.isNotFound = false;
        if (Array.isArray(agendamentos)) {
          this.agendamentos = agendamentos;
        } else {
          this.agendamentos = [agendamentos];
        }
        this.totalAgendamentos = agendamentos.length;
        this.updatePaginatedAgendamentos();
      },
      error: (err) => {
        console.error('Erro ao filtrar agendamentos:', err);

        if (err.status === 404) {
          this.isNotFound = true;
        } else {
          this.isNotFound = false;
        }

        this.agendamentos = [];
        this.totalAgendamentos = 0;
        this.updatePaginatedAgendamentos();
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePaginatedAgendamentos();
  }

  private updatePaginatedAgendamentos(): void {
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginacao = this.agendamentos.slice(startIndex, endIndex);
  }

  // ícones
  faPen = faPen;
  faTrash = faTrash;
  faMagnifyingGlass = faMagnifyingGlass;

  deletarAgendamento(id: number): void {
    this.dialog.open(ConfirmDeleteComponent).afterClosed().subscribe((result) => {
      if (result) {
        this.agendamentoService.DeletarAgendamentoPorId(id).subscribe({
          next: () => this.carregarAgendamentos(),
          error: (err) => console.error('Erro ao deletar o pet:', err)
        });
      }
    });
  }

  editarAgendamento(agendamento: any): void {
    const dialogRef = this.dialog.open(EditarAgendamentosComponent, {
      data: agendamento
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Pet editado com sucesso!');
      }
      this.carregarAgendamentos();
    });
  }


}