import { Component, inject } from '@angular/core';
import { CommonModule, AsyncPipe } from '@angular/common'; 
import { FormControl, ReactiveFormsModule } from '@angular/forms'; 
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Observable, of } from 'rxjs';
import { debounceTime, switchMap, startWith, map, catchError } from 'rxjs/operators';
import { ItemService } from '../../core/services/file/item-service';
import { CreateDialogComponent } from '../../features/item/create-dialog-component/create-dialog-component';
import { ItemCreateRequest } from '../../core/models/item-create-request';
import { TypeOfItem } from '../../core/models/enums/TypeOfItem.enum';
import { Item } from '../../core/models/Item.model';

interface DialogResult {
  type: 'folder' | 'file';
  name?: string;
  file?: File;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,      
    MatAutocompleteModule,    
    MatFormFieldModule,
    MatInputModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    AsyncPipe                 
  ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  private dialog = inject(MatDialog);
  private itemService = inject(ItemService);

  searchControl = new FormControl<string | Item>('');

  filteredOptions$: Observable<Item[]>;

  constructor() {
  this.filteredOptions$ = this.searchControl.valueChanges.pipe(
    startWith(''),
    debounceTime(300),
    switchMap(value => {
      const query = typeof value === 'string' ? value : value?.name;
      
      if (!query || !query.trim()) {
        return of<Item[]>([]);
      }

      return this.itemService.searchItems(query).pipe(
         catchError(() => of<Item[]>([])) 
      );
    })
  );
}

  onOptionSelected(event: MatAutocompleteSelectedEvent) {
    const item: Item = event.option.value;

    if (item.type === TypeOfItem.Folder) {
      this.itemService.setCurrentFolder(item.id);
    } else {
        this.itemService.getParentFolder(item.id).subscribe(parent => {
        this.itemService.setCurrentFolder(parent.id);
      });
    }

    this.searchControl.setValue('');
  }

  displayFn(item: Item): string {
    return item && item.name ? item.name : '';
  }

  onCreate() {
    const dialogRef = this.dialog.open(CreateDialogComponent, {
      width: '450px',
      data: {} 
    });

    dialogRef.afterClosed().subscribe(async (result: DialogResult | undefined) => {
      if (!result) return; 

      try {
        if (result.type === 'folder' && result.name) {
          let folder: ItemCreateRequest = {
            Name: result.name,
            Type: TypeOfItem.Folder,
            ParentId: this.itemService.currentFolderId$.value || null
          };
          await this.itemService.createFolder(folder);
        } 
        else if (result.type === 'file' && result.file) {
          await this.itemService.uploadFileFlow(result.file);
        }
      } catch (error) {
        console.error('Error during creation:', error);
      }
    });
  }

  refresh() {
    this.itemService.refresh();
  }
}