import { TypeOfItem } from './enums/TypeOfItem.enum';
import { UploadStatus } from './enums/UploadStatus.enum';

export interface ItemResponseDto {
  id: string;
  ownerId: string;
  parentId: string | null;
  type: TypeOfItem;      
  name: string;
  fileSize: number | null;
  status: UploadStatus | null;
}