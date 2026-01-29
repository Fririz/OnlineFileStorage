import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
@Component({
  selector: 'app-create-dialog-component',
  imports: [CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule],
  templateUrl: './create-dialog-component.html',
  styleUrl: './create-dialog-component.scss',
})
export class CreateDialogComponent {
selectedType: 'folder' | 'file' = 'folder';

  folderName: string = '';
  
  selectedFile: File | null = null;

  constructor(
    public dialogRef: MatDialogRef<CreateDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancel(): void {
    this.dialogRef.close(null);
  }

  onCreate(): void {
    if (this.selectedType === 'folder') {
      this.dialogRef.close({ type: 'folder', name: this.folderName });
    } else {
      this.dialogRef.close({ type: 'file', file: this.selectedFile });
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }
}
