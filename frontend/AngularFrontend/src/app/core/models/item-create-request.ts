import { Type } from "@angular/core";
import { TypeOfItem } from "./enums/TypeOfItem.enum";

export interface ItemCreateRequest {
    Name:string;
    ParentId:string | null;
    Type:TypeOfItem;
}
