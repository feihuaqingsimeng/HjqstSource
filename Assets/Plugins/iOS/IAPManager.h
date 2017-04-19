//
//  IAPManager.h
//  test
//
//  Created by 上海都玩 on 16/10/10.
//  Copyright © 2016年 上海都玩. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface IAPManager : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>{
    SKProduct *proUpgradeProduct;
    SKProductsRequest *productsRequest;
}

// 观察者
- (void)attachObserver;
// 是否开启内购
- (BOOL)CanMakePayment;
//pay success
-(void)IsPaySuccess;
// 请求商品信息(productIdentifiers 商品id)
- (void)requestProductData:(NSString *)productIdentifiers;
// 发起购买请求

- (void)buyRequest:(SKProduct *)product;
// u3d反向方法，向u3d传值，通知客户购买成功，让服务器去验证结果并且发放道具.

- (void)sendU3dMessage:(NSString *)msg ;

@end
