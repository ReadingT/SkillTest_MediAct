import { getServerSession } from "next-auth";
import { authOptions } from "@/lib/auth";
// สร้าง Client component สำหรับปุ่ม
// import ActionButtons from "@/components/ActionButtons";

type LeaveRequest = {
  id: number;
  nurseName: string;
  shiftDate: string;
  reason: string;
  status: string;
};

async function getLeaveRequests(token: string): Promise<LeaveRequest[]> {
  const res = await fetch("http://your-backend-api.com/api/leave-requests?status=Pending", {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!res.ok) return [];
  return res.json();
}

export default async function LeaveRequestsPage() {
  const session = await getServerSession(authOptions);
  // เช็คสิทธิ์อีกครั้ง (ป้องกันการเข้าถึงผ่าน URL โดยตรง)
  if (session?.user?.role !== 'HeadNurse') {
    return <p className="text-center text-red-500 mt-10">You are not authorized to view this page.</p>;
  }
  
  const requests = await getLeaveRequests(session?.accessToken ?? "");

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-3xl font-bold mb-6">รายการคำขอลา</h1>
       <div className="bg-white p-6 rounded-lg shadow-md">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left ...">ชื่อพยาบาล</th>
              <th className="px-6 py-3 text-left ...">วันที่ขอลา</th>
              <th className="px-6 py-3 text-left ...">เหตุผล</th>
              <th className="px-6 py-3 text-left ...">จัดการ</th>
            </tr>
          </thead>
          <tbody>
            {requests.map((req) => (
              <tr key={req.id}>
                <td className="px-6 py-4">{req.nurseName}</td>
                <td className="px-6 py-4">{new Date(req.shiftDate).toLocaleDateString("th-TH")}</td>
                <td className="px-6 py-4">{req.reason}</td>
                <td className="px-6 py-4">
                  <div className="flex space-x-2">
                     <button className="bg-green-500 text-white px-3 py-1 rounded">อนุมัติ</button>
                     <button className="bg-red-500 text-white px-3 py-1 rounded">ปฏิเสธ</button>
                     {/* ในการใช้งานจริง ปุ่มพวกนี้ควรเป็น Client Component ที่ยิง API
                        <ActionButtons requestId={req.id} />
                     */}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
       </div>
    </div>
  );
}