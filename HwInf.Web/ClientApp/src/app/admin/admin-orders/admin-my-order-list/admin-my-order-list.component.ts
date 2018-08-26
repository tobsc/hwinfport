import { Component, OnInit, Input } from "@angular/core";
import { OrderService } from "../../../shared/services/order.service";
import { UserService } from "../../../shared/services/user.service";
import { User } from "../../../shared/models/user.model";
import { Order } from "../../../shared/models/order.model";
import { OrderList } from "../../../shared/models/order-list.model";
import { OrderFilter } from "../../../shared/models/order-filter.model";
import { BehaviorSubject } from "rxjs";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
    selector: 'hwinf-admin-my-order-list',
    templateUrl: './admin-my-order-list.component.html',
    styleUrls: ['./admin-my-order-list.component.scss']
})
export class AdminMyOrderListComponent implements OnInit {

    public tmpFilter = new OrderFilter();
    public thisUser: User;

    public _filter = new BehaviorSubject<OrderFilter>(this.tmpFilter);
    public maxPages: number = -1;

    public orders: Order[] = [];
    public currentPage: number = 1;
    public isAscending: boolean = true;
    public totalItems: number;
    public itemsPerPage: number = 10;
    public orderBy: string = 'date';
    public order: string = "DESC";
    public maxSize: number = 8;
    public myfilter: OrderFilter = new OrderFilter();

    constructor(
        public userService: UserService,
        public orderService: OrderService,
        public route: ActivatedRoute,
        public router: Router
    ) { }

    ngOnInit() {
        this.tmpFilter.StatusSlugs = ["offen"];


        this.route.queryParams
            .flatMap(i => this._filter)
            .flatMap(filter => {
                this.currentPage = 1;
                let tmpFilter = filter;
                tmpFilter.Limit = this.itemsPerPage;
                tmpFilter.Offset = (this.currentPage - 1) * tmpFilter.Limit;
                tmpFilter.IsAdminView = true;
                tmpFilter.Order = this.order;
                tmpFilter.OrderBy = this.orderBy;

                return this.orderService.getFilteredOrders(tmpFilter);
            })
            .subscribe((data: OrderList) => {
                this.userService.getUser().subscribe(x =>
                {
                    this.thisUser = x,
                    this.orders = data.Orders.filter(x => x.Verwalter.Uid == this.thisUser.Uid);
                    this.maxPages = data.MaxPages;
                    this.totalItems = data.TotalItems;
                });


            });

    }

    fetchData() {

        this.myfilter.StatusSlugs = this._filter.value.StatusSlugs;
        this.myfilter.Order = this.order;
        this.myfilter.OrderBy = this.orderBy;
        this.myfilter.Limit = this.itemsPerPage;
        this.myfilter.IsAdminView = true;
        this.myfilter.Offset = (this.currentPage - 1) * this.myfilter.Limit;
        this.orderService.getFilteredOrders(this.myfilter)
            .subscribe(
            data => {
                this.orders = data.Orders;
                this.totalItems = data.TotalItems;
            }
            )
    }

    public pageChanged(event: any): void {
        this.currentPage = event.page;
        this.fetchData();
    }

    public onChangeOrder(orderBy: string) {
        orderBy = JSON.parse(orderBy);
        this.isAscending = !this.isAscending;
        this.myfilter.Order = orderBy['o'];
        this.order = orderBy['o'];
        this.orderBy = orderBy['by'];
        this.myfilter.OrderBy = orderBy['by'];
        this.fetchData();
    }

    updateOrder(index: number, order: Order) {
        //this.orders.splice(index,1);
        this.orders[index] = order;
    }

}

