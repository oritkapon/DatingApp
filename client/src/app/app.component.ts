import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  http = inject(HttpClient);
  title = 'client';
  users: any;

  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: (data: any) => {
        this.users = data;
      },
      error: (err: any) => {
        console.error(err);
      },
      complete: () => {
        console.log('complete');
      }
  });
}
}
