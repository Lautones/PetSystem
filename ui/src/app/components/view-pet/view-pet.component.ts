import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RippleModule } from 'primeng/ripple';
import { CadastroPetService } from '../../services/cadastro-pet.service';

@Component({
  selector: 'app-view-pet',
  imports: [
    CommonModule,
    MatDialogModule,
    ReactiveFormsModule,
    RippleModule
  ],
  templateUrl: './view-pet.component.html',
  styleUrl: './view-pet.component.css'
})
export class ViewPetComponent implements OnInit {

  viewPetForm!: FormGroup;
  especies: string[] = ['Cachorro', 'Gato'];
  racas: string[] = [];

  constructor(
    private fb: FormBuilder,
    private petService: CadastroPetService,
    private dialogRef: MatDialogRef<ViewPetComponent>,
    @Inject(MAT_DIALOG_DATA) public pet: any
  ) { }

  ngOnInit(): void {
    this.viewPetForm = this.fb.group({
      nome: [{ value: this.pet?.nome || '', disabled: true }],
      especie: [{ value: this.pet?.especie || '', disabled: true }],
      raca: [{ value: this.pet?.raca || '', disabled: true }],
      dataNascimento: [{ value: this.formatarData(this.pet?.dataNascimento) || '', disabled: true }],
      peso: [{ value: this.pet?.peso || 0, disabled: true }],
      descricao: [{ value: this.pet?.descricao || '', disabled: true }],
      cor: [{ value: this.pet?.cor || '', disabled: true }],
      tutor: [{ value: this.pet?.tutor || '', disabled: true }],
      emailTutor: [{ value: this.pet?.emailTutor || '', disabled: true }],
    });

    this.carregarRacas();
  }


  carregarRacas() {
    const especieSelecionada = this.viewPetForm.get('especie')?.value;

    if (especieSelecionada === "Cachorro") {
      this.petService.listarRacasCaes().subscribe(racasCaes => {
        this.racas = racasCaes;
      });
    } else {
      this.petService.listarRacasGatos().subscribe(racasGatos => {
        this.racas = racasGatos;
      });
    }
  }

  cancelar(): void {
    this.dialogRef.close();
  }

  formatarData(data: string): string | null {
    if (!data) return null;
    return data.split('T')[0];
  }
}