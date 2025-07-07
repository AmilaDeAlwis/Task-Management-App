import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TaskService, Task } from '../../services/task.service';

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.css']
})
export class TaskFormComponent implements OnInit, OnChanges {
  taskForm!: FormGroup;
  @Input() selectedTask: Task | null = null; // Input to receive task for editing
  @Output() taskSaved = new EventEmitter<void>(); // Emit when task is saved/updated
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService
  ) { }

  ngOnInit(): void {
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedTask'] && this.selectedTask) {
      this.patchForm();
    } else if (changes['selectedTask'] && !this.selectedTask) {
      this.resetForm(); // Reset if selectedTask becomes null (e.g., after add)
    }
  }

  initForm(): void {
    this.taskForm = this.fb.group({
      id: [null], // Hidden ID field for updates
      title: ['', Validators.required],
      description: [''],
      isCompleted: [false]
    });
  }

  patchForm(): void {
    this.clearMessages();
    if (this.selectedTask) {
      this.taskForm.patchValue({
        id: this.selectedTask.id,
        title: this.selectedTask.title,
        description: this.selectedTask.description,
        isCompleted: this.selectedTask.isCompleted
      });
    }
  }

  resetForm(): void {
    this.taskForm.reset({
      id: null,
      title: '',
      description: '',
      isCompleted: false
    });
    this.selectedTask = null; // Clear selected task reference
    this.clearMessages();
  }

  onSubmit(): void {
    this.clearMessages();
    if (this.taskForm.valid) {
      const taskData: Task = this.taskForm.value;

      if (taskData.id) {
        // Update existing task
        this.taskService.updateTask(taskData).subscribe({
          next: () => {
            this.successMessage = 'Task updated successfully!';
            this.taskSaved.emit(); // Notify parent to reload tasks
            this.resetForm(); // Clear form after update
          },
          error: (err: Error) => {
            this.errorMessage = `Failed to update task: ${err.message}`;
          }
        });
      } else {
        // Add new task
        // Remove ID from new task payload as backend generates it
        const newTaskPayload: Task = {
          title: taskData.title,
          description: taskData.description,
          isCompleted: taskData.isCompleted
        };
        this.taskService.addTask(newTaskPayload).subscribe({
          next: () => {
            this.successMessage = 'Task added successfully!';
            this.taskSaved.emit(); // Notify parent to reload tasks
            this.resetForm(); // Clear form after add
          },
          error: (err: Error) => {
            this.errorMessage = `Failed to add task: ${err.message}`;
          }
        });
      }
    } else {
      this.errorMessage = 'Please fill in all required fields.';
    }
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}