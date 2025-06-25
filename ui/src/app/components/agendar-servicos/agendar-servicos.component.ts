import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CadastroPetService } from '../../services/cadastro-pet.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { MatButtonModule } from '@angular/material/button';
import { AgendamentosService } from '../../services/agendamentos.service';

@Component({
  selector: 'app-agendar-servicos',
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ButtonModule,
    RippleModule,
    MatButtonModule
  ],
  templateUrl: './agendar-servicos.component.html',
  styleUrl: './agendar-servicos.component.css'
})

export class AgendarServicosComponent implements OnInit {

  agendamentoForm!: FormGroup;
  tipos: string[] = [
    'Vacina Polivalente', 'Vacina Antirrábica', 'Vacina Gripe Canina', 'Vacina Leishmaniose',
    'Check-up Geral', 'Consulta de Rotina', 'Consulta de Emergência', 'Exame de Sangue',
    'Banho', 'Tosa Higiênica', 'Tosa Completa', 'Hidratação de Pelagem',
    'Radiografia', 'Ultrassonografia', 'Eletrocardiograma', 'Endoscopia',
    'Podologia', 'Corte de Unhas', 'Tratamento de Calos', 'Ajuste de Almofadas Plantares',
    'Cardiologia', 'Ortopedia', 'Dermatologia', 'Odontologia',
    'Castramento', 'Cirurgia Geral', 'Microchipagem', 'Internação',
    'Remoção de Carrapatos e Pulgas', 'Terapia com Ozônio', 'Acupuntura Veterinária', 'Fisioterapia Animal',
    'Nutrição e Dietética', 'Exames de Fezes e Urina', 'Controle de Endoparasitas', 'Controle de Ectoparasitas'
  ];
  ids: string[] = [];

  constructor(private fb: FormBuilder, private petService: CadastroPetService, private agendamentoService : AgendamentosService) { }

  ngOnInit(): void {
    this.agendamentoForm = this.fb.group({
      petId: ['', Validators.required],
      tipo: ['Vacina Polivalente', Validators.required],
      data: ['', Validators.required],
      horaAgendamento: ['', Validators.required],
      observacao: ['']
    });

    this.carregarIds()
  }

  carregarIds() {
    this.petService.listarIds().subscribe(PetId => {
      this.ids = PetId;

    });
  }

  agendar(): void {
    this.agendamentoForm.markAllAsTouched();

    if (this.agendamentoForm.valid) {
      const agendamentoData = this.agendamentoForm.value;
      console.log(agendamentoData); 
      this.agendamentoService.agendar(agendamentoData).subscribe(response => {
        alert('Agendamento realizado com sucesso!');

      });

      this.limpar()
    }
  }

  limpar(): void {
    this.agendamentoForm.reset();
  }
}