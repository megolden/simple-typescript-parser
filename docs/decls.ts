
namespace MyNS {
  declare var v: any;
  declare let l: string;
  declare const c: number;
  declare function fn(): number;
  export declare type MyNumber = number;

  declare type ctp = {
    readonly p: string;
    get p2(): string;
    set p2(v: string);
    fn(): string;
  };

  declare class dc {
    p: string;
    static sp: string;
  }

  declare interface IInt {
    a: any;
  }

}

class Book {}

var mn: MyNS./*aaaaaaaa*/MyNumber;
var a: unknown = 10;
var bb = a instanceof Date;
var aa: object = {};
var bc = aa as never;
var inn = 'aa' in Date;
var to = typeof inn;
var vv = void Date
var ind = ('' as any)["00"];
var arr = ['0',0,null,{}];
var bt = typeof Book
var und = undefined;
var array = new Date();
String

var pls = null ? 0 : 1;
var pls2 = (a ?? 1 ?? 0);
var pls3 = (null && {});
var sym = Symbol("a");
var sym2 = Symbol.for("a");
type SN = string | number;
var sn: SN;
enum Alphabet {
  A = 'A',
  B = 1.5,
  C = 0.9
}
var aev: Alphabet = Alphabet.A;
type MN = number;
var mn1: MN = 1;
Object()
Boolean()
var tu: [number] = [0];

declare interface DateConstructor {
  UNow: Date;
}

type AAAA = number;
var aaA: AAAA;

var ai: Function;

var vvvv = ai(0)

declare type mtt = string|number|Date;
var typ: mtt;

var vcva = Error();
JSON.parse("")
JSON.stringify("")
var mp = new Map()
let name00: string;
name00 = null;

class CL {
  #priv: number;
  an: string;
  f() {
    this.#priv = 100;
  }
}

var ccc: CL;
ccc?.[""].toString();

"".
