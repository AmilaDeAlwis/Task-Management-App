// src/app/components/task-list/task-list.component.ts
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { TaskService, Task } from '../../services/task.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css']
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  filteredTasks: Task[] = [];
  errorMessage: string = '';
  successMessage: string = '';

  searchTerm: string = '';
  filterCompleted: string = 'all'; // 'all', 'completed', 'pending'
  sortBy: string = 'createdDate'; // 'createdDate', 'title', 'isCompleted'
  sortDirection: 'asc' | 'desc' = 'desc';

  @Output() editTask = new EventEmitter<Task>(); // Emit task to edit

  constructor(
    private taskService: TaskService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.clearMessages();
    this.taskService.getTasks().subscribe({
      next: (data) => {
        this.tasks = data;
        this.applyFiltersAndSort();
      },
      error: (err: Error) => {
        this.errorMessage = `Failed to load tasks: ${err.message}`;
      }
    });
  }

  markAsCompleted(task: Task): void {
    this.clearMessages();
    const updatedTask = { ...task, isCompleted: !task.isCompleted };
    this.taskService.updateTask(updatedTask).subscribe({
      next: () => {
        this.successMessage = `Task '${task.title}' updated successfully.`;
        this.loadTasks(); // Reload to reflect changes and re-sort/filter
      },
      error: (err: Error) => {
        this.errorMessage = `Failed to update task: ${err.message}`;
      }
    });
  }

  onEditTask(task: Task): void {
    this.editTask.emit(task); // Emit the task to the parent component (AppComponent)
    this.clearMessages();
  }

  deleteTask(id: number | undefined): void {
    this.clearMessages();
    if (id === undefined) {
      this.errorMessage = 'Cannot delete task: ID is undefined.';
      return;
    }

    if (confirm('Are you sure you want to delete this task?')) {
      this.taskService.deleteTask(id).subscribe({
        next: () => {
          this.successMessage = 'Task deleted successfully.';
          this.loadTasks(); // Reload tasks after deletion
        },
        error: (err: Error) => {
          this.errorMessage = `Failed to delete task: ${err.message}`;
        }
      });
    }
  }

  applyFiltersAndSort(): void {
    let tempTasks = [...this.tasks];

    // 1. Filter by search term
    if (this.searchTerm) {
      const lowerCaseSearch = this.searchTerm.toLowerCase();
      tempTasks = tempTasks.filter(task =>
        task.title.toLowerCase().includes(lowerCaseSearch) ||
        (task.description && task.description.toLowerCase().includes(lowerCaseSearch))
      );
    }

    // 2. Filter by completion status
    if (this.filterCompleted !== 'all') {
      const isCompleted = this.filterCompleted === 'completed';
      tempTasks = tempTasks.filter(task => task.isCompleted === isCompleted);
    }

    // 3. Sort
    tempTasks.sort((a, b) => {
      let valueA: any, valueB: any;

      switch (this.sortBy) {
        case 'title':
          valueA = a.title.toLowerCase();
          valueB = b.title.toLowerCase();
          break;
        case 'isCompleted':
          valueA = a.isCompleted ? 1 : 0;
          valueB = b.isCompleted ? 1 : 0;
          break;
        case 'createdDate':
        default:
          valueA = new Date(a.createdDate || '').getTime();
          valueB = new Date(b.createdDate || '').getTime();
          break;
      }

      if (valueA < valueB) return this.sortDirection === 'asc' ? -1 : 1;
      if (valueA > valueB) return this.sortDirection === 'asc' ? 1 : -1;
      return 0;
    });

    this.filteredTasks = tempTasks;
  }

  onSearchTermChange(): void {
    this.applyFiltersAndSort();
  }

  onFilterChange(): void {
    this.applyFiltersAndSort();
  }

  onSortChange(event: Event): void { // Accept the Event object
    const target = event.target as HTMLSelectElement; // Cast to HTMLSelectElement
    const selectedValue = target.value; // Now 'value' property is accessible

    if (this.sortBy === selectedValue) { // Use selectedValue here
        this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
        this.sortDirection = 'asc'; // Default to ascending if changing column
        this.sortBy = selectedValue; // Update sortBy to the newly selected value
    }
    this.applyFiltersAndSort();
  }

  toggleSortDirection(): void {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
      this.applyFiltersAndSort();
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  // Logout function
  logout(): void {
    this.authService.logout();
    this.tasks = []; // Clear tasks on logout
    this.filteredTasks = [];
    this.successMessage = 'You have been logged out.';
  }
}