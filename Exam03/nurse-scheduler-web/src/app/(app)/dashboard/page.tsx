// ใช้ Server Component เพื่อดึงข้อมูลก่อนแสดงผล
import { getServerSession } from "next-auth";
import { authOptions } from "@/lib/auth";
// สมมติว่าสร้าง LeaveRequestModal แยกไว้
// import LeaveRequestModal from "@/components/LeaveRequestModal";

// สร้าง Type ของข้อมูลตารางเวร
type MySchedule = {
  assignmentId: number;
  date: string;
  startTime: string;
  endTime: string;
  leaveStatus: "None" | "Pending" | "Approved" | "Rejected";
};

async function getMySchedule(token: string): Promise<MySchedule[]> {
  const res = await fetch("https://localhost:7080/schedule/my-schedule", {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!res.ok) {
    return [];
  }
  return res.json();
}

export default async function DashboardPage() {
  const session = await getServerSession(authOptions);
  const mySchedule = await getMySchedule(session?.accessToken ?? "");

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-3xl font-bold mb-6">ตารางเวรของฉัน</h1>
      <div className="bg-white p-6 rounded-lg shadow-md">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">วันที่</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">เวลาเข้าเวร</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">เวลาออกเวร</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">สถานะการลา</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">จัดการ</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {mySchedule.map((shift) => (
              <tr key={shift.assignmentId}>
                <td className="px-6 py-4">{new Date(shift.date).toLocaleDateString("th-TH")}</td>
                <td className="px-6 py-4">{shift.startTime}</td>
                <td className="px-6 py-4">{shift.endTime}</td>
                <td className="px-6 py-4">
                  <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full 
                    ${shift.leaveStatus === 'Approved' ? 'bg-green-100 text-green-800' :
                      shift.leaveStatus === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                      shift.leaveStatus === 'Rejected' ? 'bg-red-100 text-red-800' : 'bg-gray-100 text-gray-800'}`}>
                    {shift.leaveStatus}
                  </span>
                </td>
                <td className="px-6 py-4">
                  {shift.leaveStatus === 'None' && (
                    <button className="text-indigo-600 hover:text-indigo-900">
                      ขอลา
                    </button>
                    /* ในการใช้งานจริง ปุ่มนี้จะเปิด Modal
                       <LeaveRequestModal assignmentId={shift.assignmentId} />
                    */
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}