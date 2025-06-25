import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPaw, faMagnifyingGlass, faCalendar } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-home',
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ButtonModule,
    RippleModule,
    MatButtonModule,
    FontAwesomeModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})

export class HomeComponent {

  faPaw = faPaw;
  faMagnifyingGlass = faMagnifyingGlass;
  faCalendar = faCalendar;

}
