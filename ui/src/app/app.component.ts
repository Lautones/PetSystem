import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHouse, faBars, faPaw, faMagnifyingGlass, faCalendar } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatSidenavModule,
    MatIconModule,
    MatButtonModule,
    MatListModule,
    FontAwesomeModule
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  isSidebarOpen = false;
  faBars = faBars;
  faHouse = faHouse;
  faPaw = faPaw;
  faMagnifyingGlass = faMagnifyingGlass;
  faCalendar = faCalendar;
  pageTitle = 'HOME';

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.updateTitleFromUrl(event.urlAfterRedirects);
      }
    });
  }

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  closeSidebar() {
    this.isSidebarOpen = false;
  }

  private updateTitleFromUrl(url: string) {
    const routeTitles: { [key: string]: string } = {
      '/': 'HOME',
      '/cadastro': 'CADASTRAR PET',
      '/pets': 'PETS CADASTRADOS',
      '/agendar': 'AGENDAR SERVIÇO',
      '/agendamentos': 'AGENDAMENTOS'
    };

    this.pageTitle = routeTitles[url] || 'HOME';
  }
}
