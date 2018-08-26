import { Component, OnInit } from '@angular/core';
import {OrderService} from "../../../shared/services/order.service";
import {OrderFilter} from "../../../shared/models/order-filter.model";
import {Order} from "../../../shared/models/order.model";

@Component({
  selector: 'hwinf-orders-archiv',
  templateUrl: './orders-archiv.component.html',
  styleUrls: ['./orders-archiv.component.scss']
})
export class OrdersArchivComponent implements OnInit {

  public orders: Order[] = [];
  public currentPage: number = 1;
  public isAscending: boolean = true;
  public totalItems: number;
  public itemsPerPage: number = 10;
  public orderBy: string = 'orderstatus';
  public maxSize: number = 8;
  public filter: OrderFilter;
  public order: string = 'DESC';

  constructor(public orderService: OrderService) {}

  ngOnInit(): void {

    this.filter = new OrderFilter();
    this.filter.StatusSlugs = ['abgeschlossen', 'abgelehnt'];
    this.filter.Order = this.order;
    this.filter.OrderBy = this.orderBy;
    this.filter.Limit = this.itemsPerPage;
    this.filter.Offset = (this.currentPage-1) * this.filter.Limit;

    this.fetchData();
  }

  fetchData() {

    this.filter.Offset = (this.currentPage-1) * this.filter.Limit;
    this.orderService.getFilteredOrders(this.filter)
        .subscribe(
            data => {
              this.orders = data.Orders;
              this.totalItems = data.TotalItems;
            }
        )
  }

  public pageChanged(event: any): void {
    this.currentPage = event.page;
    this.fetchData()
  }

  public onChangeOrder(orderBy : string) {
    orderBy = JSON.parse(orderBy);
    this.isAscending = !this.isAscending;
    this.filter.Order = orderBy['o'];
    this.order = orderBy['o'];
    this.orderBy = orderBy['by'];
    this.filter.OrderBy = orderBy['by'];
    this.fetchData();
  }
}