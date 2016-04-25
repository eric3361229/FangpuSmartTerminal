﻿using Quartz;
using Quartz.Impl;
using System;
using System.Diagnostics;
namespace fangpu_terminal
{
    public class MySqlTableUpdate : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var entity = new FangpuDatacenterModelEntities();
            string nextmonth = DateTime.Today.AddMonths(1).ToString("yyyyMM");
            int days = DateTime.DaysInMonth(Convert.ToInt16(nextmonth.Substring(0, 4)), Convert.ToInt16(nextmonth.Substring(4, 2)));
            for (int i = 1; i <= days; i++)
            {
                string tablename = "historydata_" + nextmonth + i.ToString().PadLeft(2, '0');
                string sqlstr = "CREATE TABLE IF NOT EXISTS " + tablename + " LIKE historydata";
                int x = entity.Database.ExecuteSqlCommand(sqlstr); 
            }                    
            Trace.WriteLine("作业执行!"+DateTime.Now.ToString());
        }
    }
    public class QuartzSchedule
    {
        IScheduler sche;
        public void StartSchedule()
        {
            ISchedulerFactory sf = new StdSchedulerFactory();//执行者  
            sche= sf.GetScheduler();
            IJobDetail job1 = JobBuilder.Create<MySqlTableUpdate>()  //创建一个作业
             .WithIdentity("job1", "group1")
             .Build();
            ITrigger trigger1 = TriggerBuilder.Create()
                                       .WithIdentity("trigger1", "gruop1")
                                       .StartNow()
                                       .WithCronSchedule("0 0 0 ? * *")
                                       .Build();
            IJobDetail job2 = JobBuilder.Create<fangpu_terminal.DataAutoSync>()  //创建一个作业
            .WithIdentity("job1", "group1")
            .Build();
            ITrigger trigger2 = TriggerBuilder.Create()
                                       .WithIdentity("trigger2", "gruop1")
                                       .StartNow()
                                       .WithCronSchedule("0 0 * ? * *")
                                       .Build();
            sche.ScheduleJob(job1, trigger1);
            sche.ScheduleJob(job2, trigger2);
            sche.Start();
        }
        ~QuartzSchedule()
        {
            if(!sche.IsShutdown)
            {
                sche.Shutdown();
            }
        }
    }
}