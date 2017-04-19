//
//  IAPManager.m
//  test
//
//  Created by 上海都玩 on 16/10/10.
//  Copyright © 2016年 上海都玩. All rights reserved.
//

#import "IAPManager.h"
@implementation IAPManager
#ifdef __cplusplus
extern "C"{
#endif
    extern void UnitySendMessage(const char *,const char*,const char*);
    // 这是反向传信息给u3d的方法，内容可以自定。我这里分别是对象名SDK，方法名PaymentCallback 和 字符串 success。
    #ifdef __cplusplus
}

#endif

// 观察者
- (void)attachObserver
{
    NSLog(@"-------------attachObserver----------------");
    
    [[SKPaymentQueue defaultQueue]addTransactionObserver:self];
}

// 向Unity传递交易结果的信息
- (void)sendU3dMessage:(SKPaymentTransaction *)transaction{
    if (floor(NSFoundationVersionNumber) <= NSFoundationVersionNumber_iOS_6_1) {
        // Load resources for iOS 6.1 or earlier
    } else {
        NSURL *receiptURL = [[NSBundle mainBundle] appStoreReceiptURL];
        NSData *receipt = [NSData dataWithContentsOfURL:receiptURL];
        if(!receipt){
            NSLog(@"erorr:receipt is null");
        }
        else{
            NSLog(@"receipt:%@",receipt.base64Encoding);
            NSString *base64Encoding = receipt.base64Encoding;
            NSString *productIdentifier = transaction.payment.productIdentifier;
            NSString *transactionIdentifier = transaction.transactionIdentifier;
            NSLog(@"transactionIdentifier:%@",transactionIdentifier);
            //1、创建一个NSDictionary
            NSDictionary *dict = @{@"base64Encoding":base64Encoding,@"productIdentifier":productIdentifier,@"transactionIdentifier":transactionIdentifier};
            NSData *data =  [NSJSONSerialization dataWithJSONObject:dict options:NSJSONWritingPrettyPrinted error:nil];
            NSString *jsonData = [[NSString alloc]initWithData:data encoding:NSUTF8StringEncoding];
            //打印JSON数据
            NSLog(@"%@",jsonData);
            UnitySendMessage("base", "PaymentCallback", [jsonData UTF8String]);
        }
    }

}

// 是否支持内购
- (BOOL)CanMakePayment
{
    return [SKPaymentQueue canMakePayments];
}

// 请求商品信息
- (void)requestProductData:(NSString *)productIdentifiers
{
    NSLog(@"-------------请求对应的产品信息----------------");
    NSLog(productIdentifiers);
    NSArray * arr = [[NSArray alloc]initWithObjects:productIdentifiers, nil];
    NSSet * nsset = [NSSet setWithArray:arr];
    // NSSet无序的数组
    SKProductsRequest * request = [[SKProductsRequest alloc]initWithProductIdentifiers:nsset];
    request.delegate = self;
    [request start];
}

// 收到返回商品信息
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    NSLog(@"--------------收到产品反馈消息---------------------");
    NSArray * parr = response.products;
    
    if ([parr count] == 0) {
        NSLog(@"--------------没有商品------------------");
        return;
    }
    
    NSLog(@"productID:%@", response.invalidProductIdentifiers);
    NSLog(@"产品付费数量:%ld",[parr count]);
    
    SKProduct * p = nil;
    for (SKProduct * pro in parr)
    {
        NSLog(@"商品编号%@", [pro description]);
        NSLog(@"商品名称%@", [pro localizedTitle]);
        NSLog(@"商品介绍%@", [pro localizedDescription]);
        NSLog(@"单价%@元", [pro price]);
        NSLog(@"商品名称%@", [pro productIdentifier]);
        
        p = pro; //赋值
        
    }
    // 发起购买请求
    [self buyRequest:p];
}


// 购买请求
- (void)buyRequest:(SKProduct *)product{
    SKPayment * payment = [SKPayment paymentWithProduct:product];
    NSLog(@"payment:%ld",payment);
    [[SKPaymentQueue defaultQueue]addPayment:payment];
}


// 请求失败
- (void)request:(SKRequest *)request didFailWithError:(NSError *)error{
    NSLog(@"------------------错误-----------------:%@", error);
}



// 监听购买结果
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transaction{
    
    for(SKPaymentTransaction *tran in transaction){
        
        NSLog(@"tran的状态 %ld",(long)tran.transactionState);
        if (SKPaymentTransactionStatePurchased == tran.transactionState){
            NSLog(@"购买完成");
            //当transaction状态是 SKPaymentTransactionStatePurchased的时候，客户端就能得到一个transaction.transactionReceipt。我的目的就是要从客户端发送这个receipt，然后服务器收到receipt后，通过POST方式发送receipt到app store，app store会验证receipt并返回验证结果，服务器收到结果后再判断验证是否成功。
            
            [self completeTransaction:tran];
            
        }else if(SKPaymentTransactionStateFailed == tran.transactionState){
            
            NSLog(@"交易失败 %@",tran.payment.productIdentifier);
            NSLog(@"transaction.error.code3 %ld",tran.error.code);
            [self failedTransaction:tran];
            
        }else if(SKPaymentTransactionStateRestored == tran.transactionState){
            
            NSLog(@"恢复成功 %@",tran.payment.productIdentifier);
            [self restoreTransaction:tran];
        }
    }
}


- (void)requestDidFinish:(SKRequest *)request{
    NSLog(@"------------反馈信息结束-----------------");
}

// 交易失败
- (void)failedTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"transaction.error.code %ld",transaction.error.code);
    if(transaction.error.code != SKErrorPaymentCancelled)
    {
           NSLog(@"transaction.error.code1 %ld",transaction.error.code);
        //初始化提示框；
        UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"交易结果" message:@"购买失败！" preferredStyle:  UIAlertControllerStyleAlert];
        
        [alert addAction:[UIAlertAction actionWithTitle:@"确定" style:UIAlertActionStyleDefault handler: ^(UIAlertAction * _Nonnull action) {
            //点击按钮的响应事件；
        }]];
        
    }else {
        NSLog(@"transaction.error.code2 %ld",transaction.error.code);
        //初始化提示框；
        UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"交易结果" message:@"用户取消购买！！" preferredStyle:  UIAlertControllerStyleAlert];
        
        [alert addAction:[UIAlertAction actionWithTitle:@"确定" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            //点击按钮的响应事件；
        }]];
    }
    //[self sendU3dMessage:@"failure"];
    // 恢复购买的逻辑
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

// 恢复购买
- (void)restoreTransaction:(SKPaymentTransaction *)transaction {
    //[self sendU3dMessage:transaction];
    // 恢复已经完成的所有交易.（仅限永久有效商品）
    //[[SKPaymentQueue defaultQueue]restoreCompletedTransactions];
}

// 交易结束
- (void)completeTransaction:(SKPaymentTransaction *)transaction{
    [self sendU3dMessage:transaction];
    // 恢复购买的逻辑
    //[[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

-(void)IsPaySuccess{
    NSArray* transactions = [SKPaymentQueue defaultQueue].transactions;
    if (transactions.count > 0) {
        SKPaymentTransaction* transaction = [transactions firstObject];
        if (transaction.transactionState == SKPaymentTransactionStatePurchased) {
            NSLog(@"购买成功: %@",transaction.transactionIdentifier);
            [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
        }
    }
}
@end
