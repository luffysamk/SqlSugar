﻿using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom012
    {

        public static void Init()
        {
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables<StudentA, RoomA, SchoolA, TeacherA>();
            db.CodeFirst.InitTables<BookA>();
            db.DbMaintenance.TruncateTable<StudentA>();
            db.DbMaintenance.TruncateTable<RoomA>();
            db.DbMaintenance.TruncateTable<SchoolA>();
            db.DbMaintenance.TruncateTable<TeacherA>();
            db.DbMaintenance.TruncateTable<BookA>();
            db.Insertable(new RoomA() { RoomId = 1, RoomName = "北大001室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 2, RoomName = "北大002室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 3, RoomName = "北大003室", SchoolId = 1 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 4, RoomName = "清华001厅", SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 5, RoomName = "清华002厅", SchoolId = 2 }).ExecuteCommand();
            db.Insertable(new RoomA() { RoomId = 6, RoomName = "清华003厅", SchoolId = 2 }).ExecuteCommand();


            db.Insertable(new SchoolA() { SchoolId = 1, CityId= 1001001, School_Name = "北大" }).ExecuteCommand();
            db.Insertable(new SchoolA() { SchoolId = 2 , CityId=2,School_Name = "清华" }).ExecuteCommand();
            db.Insertable(new SchoolA() { SchoolId = 3, CityId = 3, School_Name = "青鸟" }).ExecuteCommand();

            db.Insertable(new StudentA() { StudentId = 1, SchoolId = 1, Name = "北大jack" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 2, SchoolId = 1, Name = "北大tom" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 3, SchoolId = 2, Name = "清华jack" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 4, SchoolId = 2, Name = "清华tom" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 5, SchoolId = null, Name = "清华tom" }).ExecuteCommand();
            db.Insertable(new StudentA() { StudentId = 6, SchoolId = 3, Name = "青鸟学生" }).ExecuteCommand();
            db.Insertable(new TeacherA() { SchoolId = 1, Id = 1, Name = "北大老师01" }).ExecuteCommand();
            db.Insertable(new TeacherA() { SchoolId = 1, Id = 2, Name = "北大老师02" }).ExecuteCommand();

            db.Insertable(new TeacherA() { SchoolId = 2, Id = 3, Name = "清华老师01" }).ExecuteCommand();
            db.Insertable(new TeacherA() { SchoolId = 2, Id = 4, Name = "清华老师02" }).ExecuteCommand();


            db.Insertable(new BookA() { BookId = 1, Names = "java", studenId = 1 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 2, Names = "c#2", studenId = 2 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 3, Names = "c#1", studenId = 2 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 4, Names = "php", studenId = 3 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 5, Names = "js", studenId = 4 }).ExecuteCommand();
            db.Insertable(new BookA() { BookId = 6, Names = "北大jack", studenId = 1 }).ExecuteCommand();

            var list2 = db.Queryable<StudentA>()
           .Includes(x => x.SchoolA, x => x.RoomList)//2个参数就是 then Include 
           .Includes(x => x.SchoolA, x => x.TeacherList)//2个参数就是 then Include 
           .Includes(x => x.Books)
           .Where(x => x.Books.Any(z => z.BookId == 1))
           .Where(x => x.SchoolA.School_Name.Contains("北大"))
           .ToList();

            var list21 = db.Queryable<StudentA>()
                 .Mapper(it => it.SchoolA, it => it.SchoolId)
                 .ToList();

            var list22 = db.Queryable<StudentA>()
                 //.Includes(it => it.SchoolA)
                 .Where(it=>it.SchoolA.City.Id== 1001001)
                 .ToList();
            var list33 = db.Queryable<StudentA>()
            //.Includes(it => it.SchoolA)
            .Where(it => it.SchoolA.SchoolId == 1)
            .ToList();

            Check.Exception(string.Join(",", list22.Select(it => it.StudentId)) != string.Join(",", list33.Select(it => it.StudentId)), "unit error");


            var list333 = db.Queryable<StudentA>()
               .Includes(it => it.SchoolA,it=>it.TeacherList)
               .Where(it => it.SchoolA.TeacherList.Any())
               .ToList();

            var xxxx = 0;
            var pageList = db.Queryable<StudentA>()
               .Includes(it => it.SchoolA, it => it.TeacherList)
               .Where(it => it.SchoolA.TeacherList.Any())
               .ToPageList(1, 2, ref xxxx);

            var list3333 = db.Queryable<StudentA>()
                .Includes(it => it.SchoolA, it => it.TeacherList)
                .Where(it => it.SchoolA.TeacherList.Any(z=>z.Id>2))
                .ToList();

            Check.Exception(list3333.Select(x=>x.SchoolA).SelectMany(x=>x.TeacherList).Any(it=>it.Id<=2), "unit error");

            var list3 = db.Queryable<StudentA>()
           .Includes(x => x.SchoolA, x => x.RoomList)//2个参数就是 then Include 
           .Includes(x => x.SchoolA, x => x.TeacherList)//2个参数就是 then Include 
           .Includes(x => x.Books.Select(z=>new BookA() { Names = z.Names }).ToList())
           .Where(x => x.Books.Any(z => z.BookId == 1))
           .Where(x => x.SchoolA.School_Name == "北大")
           .ToList();



            var list3_1 = db.Queryable<StudentA>()
           .Includes(x => x.Books.MappingField(z=>z.Names,()=>x.Name).ToList())
           .ToList();

            //先用Mapper导航映射查出第二层
            var list = db.Queryable<StudentA>().Mapper(x => x.SchoolA, x => x.SchoolId).ToList();

            //参数1 ：将第二层对象合并成一个集合  参数2：委托
            //说明：如果2级对象是集合用SelectMany
            db.ThenMapper(list.Select(it => it.SchoolA), sch =>
            {
                //参数1: room表关联字段  参数2: school表关联字段，  参数3: school当前记录
                if(sch!=null)
                  sch.RoomList = db.Queryable<RoomA>().SetContext(room => room.SchoolId, () => sch.SchoolId, sch).ToList();
                if (sch != null)
                    sch.TeacherList = db.Queryable<TeacherA>().SetContext(teachera => teachera.SchoolId, () => sch.SchoolId, sch).ToList();
            });
     

       
            db.CodeFirst.InitTables<A1, B1, ABMapping1>();
            db.DbMaintenance.TruncateTable<A1>();
            db.DbMaintenance.TruncateTable<B1>();
            db.DbMaintenance.TruncateTable<ABMapping1>();
            db.Insertable(new A1() { Id = 1, Name = "a1" }).ExecuteCommand();
            db.Insertable(new A1() { Id = 2, Name = "a2" }).ExecuteCommand();
            db.Insertable(new A1() { Id = 3, Name = "a3" }).ExecuteCommand();
            db.Insertable(new B1() { Id = 1, Name = "b1" }).ExecuteCommand();
            db.Insertable(new B1() { Id = 2, Name = "b2" }).ExecuteCommand();
            db.Insertable(new ABMapping1() {  AId=1,BId=1 }).ExecuteCommand();
            db.Insertable(new ABMapping1() { AId =2, BId = 1 }).ExecuteCommand();
            db.Insertable(new ABMapping1() { AId = 2, BId = 2 }).ExecuteCommand();
            var p = "";
            var list4= db.Queryable<A1>()
                .Includes(x => x.BList.WhereIF(!string.IsNullOrEmpty(p),it=>it.Id==11).ToList())
                .Where(x=>x.BList.Any()).ToList();

            var list31 = db.Queryable<A1>().Includes(x => x.BList,x=>x.AList).ToList();

            db.CodeFirst.InitTables(typeof(Tree1));
            db.DbMaintenance.TruncateTable("Tree1");
            db.Insertable(new Tree1() { Id = 1, Name = "01" }).ExecuteCommand();
            db.Insertable(new Tree1() { Id = 2, Name = "0101", ParentId = 1 }).ExecuteCommand();
            db.Insertable(new Tree1() { Id = 3, Name = "0102", ParentId = 1 }).ExecuteCommand();
            db.Insertable(new Tree1() { Id = 4, Name = "02" }).ExecuteCommand();
            db.Insertable(new Tree1() { Id = 5, Name = "0201", ParentId = 2 }).ExecuteCommand(); 
            db.Insertable(new Tree1() { Id = 6, Name = "020101", ParentId = 5 }).ExecuteCommand();
            db.Insertable(new Tree1() { Id = 7, Name = "02010101", ParentId = 6 }).ExecuteCommand();

            db.Queryable<Tree1>().Where(x => x.Parent.Parent.Parent.Id > 0).ToList();
            db.Queryable<Tree1>().Where(x => x.Parent.Child.Any()).ToList();
            var list21111 = new List<Tree1>();
           var xxx= db.Queryable<Tree1>()
                .Includes(it => it.Child,it=>it.Child,it=>it.Child)
                .Includes(it => it.Parent,it=>it.Parent,it=>it.Parent)
                //.Where(it=>it.Child.Any())
                .ToList();

            db.ThenMapper(xxx, it =>
            {
                it.Child = it.Child.OrderBy(x => x.Id).ToList();
            });
            //var json = db.Utilities.SerializeObject(list4);

            db.CodeFirst.InitTables<UnitA001, UnitA002>();
            db.DbMaintenance.TruncateTable<UnitA001>();
            db.DbMaintenance.TruncateTable<UnitA002>();

            db.Insertable(new UnitA001() { id = 1, name1 = "a", orgid = "1" }).ExecuteCommand();
            db.Insertable(new UnitA002() { id = 1, name2= "a2", orgid = "1" }).ExecuteCommand();
            var list5=db.Queryable<UnitA001>().ToList();
            db.ThenMapper(list5, it =>
            {
                it.UnitA002 = db.Queryable<UnitA002>().SetContext(x => x.orgid, () => it.id, it).First();
            });
            db.Queryable(db.Queryable<Order>())
                .Select(it => new
                {
                    x = SqlFunc.Subqueryable<Order>().Where(z => z.Id == it.Id).Any()
                }).ToList();
        }

        public class UnitA001
        {
            public int id { get; set; }
            public string name1 { get; set; }
            public string orgid { get; set; }
            [SugarColumn(IsIgnore =true)]
            public UnitA002 UnitA002 { get; set; }
        }
        public class UnitA002
        {
            public int id { get; set; }
            public string name2{ get; set; }
            public string orgid { get; set; }
        }
        public class Tree1
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int ParentId { get; set; }
            [Navigate(NavigateType.OneToOne,nameof(ParentId))]
            public Tree1 Parent { get; set; }
            [Navigate(NavigateType.OneToMany,nameof(Tree1.ParentId))]
            public List<Tree1> Child { get; set; }
        }
        public class ABMapping1
        {
            [SugarColumn(IsPrimaryKey = true )]
            public int AId { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public int BId { get; set; }
        }
        public class A1
        {
            [SugarColumn(IsPrimaryKey = true  )]
            public int Id { get; set; }
            public string Name { get; set; }
            [Navigate(typeof(ABMapping1),nameof(ABMapping1.AId),nameof(ABMapping1.BId))]
            public List<B1> BList { get; set; }
        }
        public class B1
        {
            [SugarColumn(IsPrimaryKey = true )]
            public int Id { get; set; }
            public string Name { get; set; }
            [Navigate(typeof(ABMapping1), nameof(ABMapping1.BId), nameof(ABMapping1.AId))]
            public List<A1> AList { get; set; }
        }

        public class StudentA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int StudentId { get; set; }
            public string Name { get; set; }
            [SugarColumn(IsNullable =true)]
            public int? SchoolId { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(SchoolId))]
            public SchoolA SchoolA { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(BookA.studenId))]
            public List<BookA> Books { get; set; }

        }
        public class SchoolA
        {
            [SugarColumn(IsPrimaryKey = true,ColumnName = "schoolid")]
            public int SchoolId { get; set; }
            [SugarColumn(IsNullable =true)]
            public int CityId { get; set; }
            [SugarColumn( ColumnName = "SchoolName")]
            public string School_Name { get; set; }
            [Navigate(NavigateType.OneToMany,nameof(RoomA.SchoolId))]
            public List<RoomA> RoomList { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(TeacherA.SchoolId))]
            public List<TeacherA> TeacherList { get; set; }
            [Navigate(NavigateType.OneToOne,nameof(CityId))]
            public City City { get; set; }

        }
        public class TeacherA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            public int SchoolId { get; set; }
            public string Name { get; set; }
        }
        public class RoomA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int RoomId { get; set; }
            public string RoomName { get; set; }
            public int SchoolId { get; set; }
        }
        public class BookA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int BookId { get; set; }
            [SugarColumn(ColumnName ="Name")]
            public string Names{ get; set; }
            public int studenId { get; set; }
        }

    }
}
