
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
  A = 0,
  B = 1.5,
  C = 0.9
}
var aev: Alphabet = 10;
var aevi: number = Alphabet.A;

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
var typ: mtt = 0 as any;

var arran: any[] = [10,'a'];
var 


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

// ---------------------------------------

declare var DateTime: DateConstructor;
declare interface Guid {
  parts: number;
};
declare interface GuidConstructor {
  new(v:string): Guid;
  (v: string): string;
};
declare var Guid: GuidConstructor;
const abb: bigint = 0n;

interface II {
  readonly na: number;
}
interface II extends Function {
  /** @deprecated */
  readonly na: number;
}
var VII: II = null as any;

const aVI = (null as any) instanceof VII;

enum EEE {
  a = null,
  b = 10
}

type X1 = number;

const x1: X1 = 0;
const n: number = x1;

const s = String('a').trim();
const ss: String = new String('');

const d = Date();
const dd = new Date();
const g = new Guid("");
