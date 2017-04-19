//
//  IAPInterface.m
//  test
//
//  Created by 上海都玩 on 16/10/10.
//  Copyright © 2016年 上海都玩. All rights reserved.
//

#import "IAPInterface.h"

#import "IAPManager.h"
@implementation IAPInterface
#ifdef __cplusplus
extern "C"{
#endif
    IAPManager *iapManager = nil;
    void IOSPayment (void * p)      //接口
    {
        if (iapManager == nil)
        {
            iapManager  = [[IAPManager alloc]init];
        }
        // 开始注册观察者
        //[iapManager attachObserver];
        
        // 如果已经开启内购
        if (IsProductAvailable()) {
            // 开始执行请求商品行为
            RequstProductInfo(p);
        }else{
            NSLog(@"不允许程序内付费");
        }
    }
    
    void InitPayment(){
        if (iapManager == nil)
        {
            iapManager  = [[IAPManager alloc]init];
        }
        // 开始注册观察者
        [iapManager attachObserver];
    }
    // 返回玩家是否开启IAP内购
    bool IsProductAvailable()
    {
        return [iapManager CanMakePayment];
    }
    // 输入商品key列表 获取商品信息
    void RequstProductInfo(void * p)
    {
        // UF8String是因为 C++的字符跟OC的字符不同。
        NSString * pa = [NSString stringWithUTF8String:p];
        // 因为接收到的商品id --> p 不完整，需要拼接。
        //NSString * str = @"com.LKPY.bundleId.";
        //（注意有个点，因为在iTunesConnet上，我的商品ID是com.LKPY.bundleId.xxxx ，这里的xxxx是服务          器发给我的商品ID字符串）
        //NSString * Product = [str stringByAppendingString:pa];
        
        // 开始请求商品
        [iapManager requestProductData:pa]; 
    }
    
    void VerifySuccess(){
        [iapManager IsPaySuccess];
    }
#ifdef __cplusplus
}

#endif
@end
