import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { CadastroPetComponent } from './cadastro-pet.component';
import { CadastroPetService } from '../../services/cadastro-pet.service';
import { of } from 'rxjs';

describe('CadastroPetComponent', () => {
  let component: CadastroPetComponent;
  let fixture: ComponentFixture<CadastroPetComponent>;
  let mockPetService: jasmine.SpyObj<CadastroPetService>;

  beforeEach(async () => {
    mockPetService = jasmine.createSpyObj('CadastroPetService', [
      'listarRacasCaes',
      'listarRacasGatos',
      'cadastrarPet',
    ]);

    await TestBed.configureTestingModule({
      declarations: [CadastroPetComponent],
      imports: [ReactiveFormsModule],
      providers: [{ provide: CadastroPetService, useValue: mockPetService }],
    }).compileComponents();

    fixture = TestBed.createComponent(CadastroPetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('Deve criar o formulário com os campos esperados', () => {
    expect(component.cadastroPetForm.contains('nome')).toBeTrue();
    expect(component.cadastroPetForm.contains('especie')).toBeTrue();
    expect(component.cadastroPetForm.contains('raca')).toBeTrue();
    expect(component.cadastroPetForm.contains('dataNascimento')).toBeTrue();
    expect(component.cadastroPetForm.contains('peso')).toBeTrue();
    expect(component.cadastroPetForm.contains('descricao')).toBeTrue();
    expect(component.cadastroPetForm.contains('cor')).toBeTrue();
    expect(component.cadastroPetForm.contains('tutor')).toBeTrue();
    expect(component.cadastroPetForm.contains('emailTutor')).toBeTrue();
  });

  it('Deve carregar raças de cachorro quando especie for "Cachorro"', () => {
    component.cadastroPetForm.get('especie')?.setValue('Cachorro');
    mockPetService.listarRacasCaes.and.returnValue(of(['Labrador', 'Poodle']));

    component.carregarRacas();

    expect(mockPetService.listarRacasCaes).toHaveBeenCalled();
  });

  it('Deve carregar raças de gato quando especie for "Gato"', () => {
    component.cadastroPetForm.get('especie')?.setValue('Gato');
    mockPetService.listarRacasGatos.and.returnValue(of(['Persa', 'Siamês']));

    component.carregarRacas();

    expect(mockPetService.listarRacasGatos).toHaveBeenCalled();
  });

  it('Deve chamar o service de cadastro se o formulário for válido', () => {
    component.cadastroPetForm.setValue({
      nome: 'Rex',
      especie: 'Cachorro',
      raca: 'Labrador',
      dataNascimento: '2020-01-01',
      peso: 10,
      descricao: 'Pet feliz',
      cor: 'Marrom',
      tutor: 'João',
      emailTutor: 'joao@email.com',
    });

    mockPetService.cadastrarPet.and.returnValue(of({}));

    component.cadastrar();

    expect(mockPetService.cadastrarPet).toHaveBeenCalledWith(
      component.cadastroPetForm.value
    );
  });

  it('Deve resetar o formulário e limpar raças ao cancelar', () => {
    component.racas = ['Labrador'];
    component.cancelar();
    expect(component.cadastroPetForm.pristine).toBeTrue();
    expect(component.racas.length).toBe(0);
  });
});