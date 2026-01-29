import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Item } from '../../../core/models/Item.model';
import { TypeOfItem } from '../../../core/models/enums/TypeOfItem.enum';
import { FileSizePipe } from '../../../shared/pipes/file-size-pipe';

import { MatCardModule } from '@angular/material/card'; 
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button'; 

@Component({
  selector: 'app-item-card',
  templateUrl: './item-card.html',
  styleUrls: ['./item-card.scss'],
  standalone: true,
  imports: [
    FileSizePipe, 
    MatCardModule, 
    MatIconModule,
    MatButtonModule
  ]
})
export class ItemCard {
  protected readonly TypeOfItem = TypeOfItem;
  @Input() item!: Item;

  @Output() download = new EventEmitter<Item>();
  @Output() delete = new EventEmitter<Item>();
  @Output() openFolder = new EventEmitter<Item>();

  onDownloadClick(event: Event) {
    event.stopPropagation(); 
    this.download.emit(this.item);
  }

  onDeleteClick(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.item);
  }

  openFolderClick() {
    console.log('Card Clicked:', this.item.name); 
    this.openFolder.emit(this.item);
  }
}