﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SakuraBridge.Base
{
    /// <summary>
    /// module dll (アセンブリ) 内からモジュールクラスを探し出すクラスのインターフェース
    /// </summary>
    public interface IModuleClassFinder
    {
        /// <summary>
        /// module dll (アセンブリ) 内から全モジュールクラスの名前を取得
        /// </summary>
        string[] GetModuleClassFullNames(string moduleAssemblyPath);
    }

    /// <summary>
    /// module dll (アセンブリ) 内からモジュールクラスを探し出すクラス (モジュールドメイン下で実行される)
    /// </summary>
    public class ModuleClassFinder : MarshalByRefObject, IModuleClassFinder
    {
        /// <summary>
        /// module dll (アセンブリ) 内からモジュールクラスを探し出し、配列として返す
        /// </summary>
        public virtual string[] GetModuleClassFullNames(string moduleAssemblyPath)
        {
            Assembly asm = Assembly.LoadFrom(moduleAssemblyPath);
            string iModuleName = typeof(IModule).FullName;
            List<string> moduleFullNames = new List<string>();
            foreach (Type t in asm.GetTypes())
            {
                // アセンブリ内のすべての型について、publicなModuleクラスであるかどうかを調べる
                if (t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(iModuleName) != null)
                {
                    // Moduleクラスであれば、その名前を追加
                    moduleFullNames.Add(t.FullName);
                }
            }

            // 見つかった名前をすべて返す
            return moduleFullNames.ToArray();
        }
    }
}
