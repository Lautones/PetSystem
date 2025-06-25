//Eu deveria ter feito um formulário genérico para poder ser aproveitado em outras partes do código

import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { CadastroPetService } from '../../services/cadastro-pet.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-cadastro-pet',
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ButtonModule,
    RippleModule,
    MatButtonModule
  ],
  templateUrl: './cadastro-pet.component.html',
  styleUrl: './cadastro-pet.component.css'

})

export class CadastroPetComponent implements OnInit {
  cadastroPetForm!: FormGroup;
  especies: string[] = ['Cachorro', 'Gato'];
  racas: string[] = [];

  constructor(private fb: FormBuilder, private petService: CadastroPetService) { }

  ngOnInit(): void {
    this.cadastroPetForm = this.fb.group({
      nome: ['', Validators.required],
      especie: ['Cachorro', Validators.required],
      raca: ['', Validators.required],
      dataNascimento: ['', Validators.required],
      peso: ['', [Validators.required, Validators.min(0)]],
      descricao: [''],
      cor: ['', Validators.required],
      tutor: ['', Validators.required],
      emailTutor: ['', [Validators.required, Validators.email]],

    });

    this.carregarRacas();

  }

  carregarRacas() {
    const especieSelecionada = this.cadastroPetForm.get('especie')?.value;

    if (especieSelecionada == "Cachorro") {
      this.petService.listarRacasCaes().subscribe(racasCaes => {
        this.racas = racasCaes;

      });

    }
    else {
      this.petService.listarRacasGatos().subscribe(racasGatos => {
        this.racas = racasGatos;
        
      });
    }
  }

  cadastrar(): void {
    this.cadastroPetForm.markAllAsTouched();

    if (this.cadastroPetForm.valid) {
      const petData = this.cadastroPetForm.value;

      this.petService.cadastrarPet(petData).subscribe(response => {
        alert('Pet cadastrado com sucesso!');
      });

      this.cancelar()
    }
  } 

  cancelar(): void {
    this.cadastroPetForm.reset();
    this.racas = [];
  }
}