import { Routes } from '@angular/router';
import { CadastroPetComponent } from './components/cadastro-pet/cadastro-pet.component';
import { HomeComponent } from './components/home/home.component';
import { PetsComponent } from './components/pets/pets.component';
import { AgendarServicosComponent } from './components/agendar-servicos/agendar-servicos.component';
import { AgendamentosComponent } from './components/agendamentos/agendamentos.component';

export const routes: Routes = [
    {path:'cadastro', component: CadastroPetComponent},
    {path:'', component: HomeComponent},
    {path:'pets', component: PetsComponent},
    {path:'agendar', component: AgendarServicosComponent},
    {path:'agendamentos', component: AgendamentosComponent}
    
];
