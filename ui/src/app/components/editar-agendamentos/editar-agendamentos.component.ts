import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { AgendamentosService } from '../../services/agendamentos.service';
import { CadastroPetService } from '../../services/cadastro-pet.service';

@Component({
  selector: 'app-editar-agendamentos',
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    ReactiveFormsModule,
    ButtonModule,
    RippleModule
  ],
  templateUrl: './editar-agendamentos.component.html',
  styleUrl: './editar-agendamentos.component.css'
})
export class EditarAgendamentosComponent implements OnInit {

  agendamentoEditForm!: FormGroup;
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

  constructor(
    private fb: FormBuilder,
    private petService: CadastroPetService,
    private agendamentoService: AgendamentosService,
    private dialogRef: MatDialogRef<EditarAgendamentosComponent>,
    @Inject(MAT_DIALOG_DATA) public agendamento: any
  ) { }

  ngOnInit(): void {
    this.agendamentoEditForm = this.fb.group({
      petId: [this.agendamento?.petId || '', Validators.required],
      tipo: [this.agendamento?.tipo || '', Validators.required],
      data: [this.agendamento?.data || '', Validators.required],
      horaAgendamento: [this.agendamento?.horaAgendamento || ''],
      observacao: [this.agendamento?.observacao || '']
    });









    this.carregarIds()
  }

  carregarIds() {
    this.petService.listarIds().subscribe(PetId => {
      this.ids = PetId;

    });
  }


  editar(): void {
    if (this.agendamentoEditForm.valid) {
      const agendamentoData = this.agendamentoEditForm.value;
      this.agendamentoService.editarAgendamento(this.agendamento.id, agendamentoData).subscribe(response => {
        this.dialogRef.close(true);
      });
    } else {
      this.agendamentoEditForm.markAllAsTouched();
    }
  }

  cancelar(): void {
    this.dialogRef.close();
  }


}
