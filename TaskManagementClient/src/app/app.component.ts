// src/app/app.component.ts
import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Task } from './services/task.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'TaskManagementApp';
  isLoggedIn: boolean = false;
  selectedTaskForEdit: Task | null = null; // Property to hold the task being edited

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    // Subscribe to login status changes from AuthService
    this.authService.isLoggedIn().subscribe(status => {
      this.isLoggedIn = status;
      // If user logs out, clear selected task
      if (!status) {
        this.selectedTaskForEdit = null;
      }
    });
  }

  onTaskSaved(): void {
    // This method is called when TaskFormComponent saves/updates a task.
    // It signals TaskListComponent to reload tasks.
    this.selectedTaskForEdit = null;
  }

  onEditTask(task: Task): void {
    // This method is called when TaskListComponent emits a task for editing.
    this.selectedTaskForEdit = task;
  }
}