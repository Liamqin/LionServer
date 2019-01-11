using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using CYQ.Data.Table;

namespace CYQ.Data.Tool
{
    /// <summary>
    /// ����ת����[������Խ��[500����],����Խ��]
    /// </summary>
    internal class FastToT<T>
    {
        public delegate T EmitHandle(MDataRow row);
        /// <summary>
        /// ����һ��ORMʵ��ת����
        /// </summary>
        /// <typeparam name="T">ת����Ŀ������</typeparam>
        /// <param name="schema">�����ݼܹ�</param>
        public static EmitHandle Create(MDataTable schema)
        {
            Type tType = typeof(T);
            Type rowType = typeof(MDataRow);
            DynamicMethod method = new DynamicMethod("RowToT", tType, new Type[] { rowType }, tType);


            MethodInfo getValue = rowType.GetMethod("GetItemValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);


            ILGenerator gen = method.GetILGenerator();//��ʼ��дIL������

            gen.DeclareLocal(tType);
            gen.DeclareLocal(typeof(object));
            gen.DeclareLocal(typeof(bool)); //�ֱ�����һ��Type t,object o,bool b;

            gen.Emit(OpCodes.Newobj, tType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { }, null));
            gen.Emit(OpCodes.Stloc_0);//t= new T();
            int ordinal = -1;

            List<FieldInfo> fileds = new List<FieldInfo>();
            fileds.AddRange(tType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            if (tType.BaseType.Name != "Object" && tType.BaseType.Name != "OrmBase")
            {
                FieldInfo[] items = tType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (FieldInfo item in items)
                {
                    bool isAdd = true;
                    foreach (FieldInfo f in fileds)//������ȥ�ظ���
                    {
                        if (item.Name == f.Name)
                        {
                            isAdd = false;
                            break;
                        }
                    }
                    
                    if (isAdd)
                    {
                        fileds.Add(item);
                    }
                }
            }
            foreach (FieldInfo field in fileds)
            {
                string fieldName = field.Name;
                if(fieldName[0]=='<')
                {
                    fieldName=fieldName.Substring(1,fieldName.IndexOf('>')-1);
                }
                ordinal = schema.Columns.GetIndex(fieldName.TrimStart('_'));
                if (ordinal == -1)
                {
                    ordinal = schema.Columns.GetIndex(fieldName);
                }
                if (ordinal > -1)
                {
                    Label retFalse = gen.DefineLabel();//�����ǩ��goto;
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldc_I4, ordinal);
                    gen.Emit(OpCodes.Call, getValue);//Call GetItemValue(ordinal);
                    gen.Emit(OpCodes.Stloc_1); // o=GetItemValue(ordinal);

                    gen.Emit(OpCodes.Ldloc_1);
                    gen.Emit(OpCodes.Ldnull);
                    gen.Emit(OpCodes.Ceq);// if (o==null)
                    gen.Emit(OpCodes.Stloc_2); //b=o==null;
                    gen.Emit(OpCodes.Ldloc_2);

                    gen.Emit(OpCodes.Brtrue_S, retFalse);//Ϊnullֵ������

                    gen.Emit(OpCodes.Ldloc_0);//ʵ�����
                    gen.Emit(OpCodes.Ldloc_1);//���Ե�ֵ
                    EmitCastObj(gen, field.FieldType);//����ת��
                    gen.Emit(OpCodes.Stfld, field);//��ʵ�帳ֵ System.Object.FieldSetter(String typeName, String fieldName, Object val)

                    gen.MarkLabel(retFalse);//������һ��ѭ��
                }
            }

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof(EmitHandle)) as EmitHandle;
        }

        private static void EmitCastObj(ILGenerator il, Type targetType)
        {
            if (targetType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, targetType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, targetType);
            }
        }
    }
}
