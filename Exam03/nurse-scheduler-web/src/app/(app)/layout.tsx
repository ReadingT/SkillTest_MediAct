import { getServerSession } from "next-auth";
import Link from "next/link";
import { authOptions } from "@/lib/auth";

export default async function AppLayout({ children }: { children: React.ReactNode }) {
  const session = await getServerSession(authOptions);
  const isHeadNurse = session?.user?.role === 'HeadNurse';

  return (
    <div>
      <nav className="bg-white shadow-md p-4">
        <div className="container mx-auto flex justify-between">
            <Link href="/dashboard" className="font-bold">Home</Link>
            <div>
                {isHeadNurse && (
                    <>
                        <Link href="/schedule-management" className="mr-4">จัดการตารางเวร</Link>
                        <Link href="/leave-requests">จัดการใบลา</Link>
                    </>
                )}
            </div>
        </div>
      </nav>
      <main>{children}</main>
    </div>
  );
}