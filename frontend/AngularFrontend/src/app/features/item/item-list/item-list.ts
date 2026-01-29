import { Component, OnInit, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Item } from '../../../core/models/Item.model';
import { ItemService } from '../../../core/services/file/item-service';
import { ItemCard } from '../item-card/item-card';
import { MatIcon } from '@angular/material/icon';
import { Observable } from 'rxjs';
import { TypeOfItem } from '../../../core/models/enums/TypeOfItem.enum';
import { MatButtonModule } from "@angular/material/button"; 

@Component({
  selector: 'app-item-list',
  standalone: true,
  imports: [ItemCard, AsyncPipe, MatIcon, MatButtonModule],
  templateUrl: './item-list.html',
  styleUrl: './item-list.scss',
})
export class ItemList implements OnInit {
  public itemService = inject(ItemService);

  files$!: Observable<Item[]>;

  ngOnInit() {
    this.files$ = this.itemService.getCurrentFolderFiles();
  }

  openFolder(folder: Item) {
    if (folder.type === TypeOfItem.Folder) {
      this.itemService.setCurrentFolder(folder.id);
    }
  }

  goUp() {
    this.itemService.goUp();
  }

  async deleteItem(item: Item) {
    if (item.type === TypeOfItem.File) {
      await this.itemService.deleteFile(item.id);
    } else if (item.type === TypeOfItem.Folder) {
      this.itemService.deleteFolder(item.id);
    }
    await this.itemService.refresh();
  }

  async downloadFile(file: Item) {
    let url = await this.itemService.getDownloadUrl(file.id);
    window.open(url, '_self');
  }
}