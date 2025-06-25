import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { CadastroPetService } from '../../services/cadastro-pet.service';

@Component({
  selector: 'app-editar-pet',
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    ReactiveFormsModule,
    ButtonModule,
    RippleModule
  ],
  templateUrl: './editar-pet.component.html',
  styleUrl: './editar-pet.component.css'
})
export class EditarPetComponent implements OnInit {

  editPetForm!: FormGroup;
  especies: string[] = ['Cachorro', 'Gato'];
  racas: string[] = [];
  
  constructor(
    private fb: FormBuilder, 
    private petService: CadastroPetService,
    private dialogRef: MatDialogRef<EditarPetComponent>,
    @Inject(MAT_DIALOG_DATA) public pet: any  
  ) { }

  ngOnInit(): void {
    this.editPetForm = this.fb.group({
      nome: [this.pet?.nome || '', Validators.required],
      especie: [this.pet?.especie || '', Validators.required],
      raca: [this.pet?.raca || '', Validators.required],
      dataNascimento: [this.formatarData(this.pet?.dataNascimento) || '', Validators.required],
      peso: [this.pet?.peso || 0, [Validators.required, Validators.min(0)]],
      descricao: [this.pet?.descricao || ''],
      cor: [this.pet?.cor || '', Validators.required],
      tutor: [this.pet?.tutor || '', Validators.required],
      emailTutor: [this.pet?.emailTutor || '', [Validators.required, Validators.email]],
    });

    this.carregarRacas();
  }

  carregarRacas() {
    const especieSelecionada = this.editPetForm.get('especie')?.value;

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

  editar(): void {
    if (this.editPetForm.valid) {
      const petData = this.editPetForm.value;
      this.petService.editarPet(this.pet.id, petData).subscribe(response => {
        this.dialogRef.close(true); 
      });
    } else {
      this.editPetForm.markAllAsTouched();
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

