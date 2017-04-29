import { Component, OnInit } from '@angular/core';
import {OrderService} from "../../../shared/services/order.service";
import {Order} from "../../../shared/models/order.model";
import {OrderFilter} from "../../../shared/models/order-filter.model";

@Component({
  selector: 'hwinf-my-orders',
  templateUrl: './my-orders.component.html',
  styleUrls: ['./my-orders.component.scss']
})
export class MyOrdersComponent implements OnInit {

  private orders: Order[];

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {

    let filter = new OrderFilter();
    filter.StatusSlugs = ['offen', 'akzeptiert', 'ausgeliehen'];

    this.orderService.getFilteredOrders(filter)
        .subscribe(
            data => { this.orders = data.Orders; }
        )
  }

}
